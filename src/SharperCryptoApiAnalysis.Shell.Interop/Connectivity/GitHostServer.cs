using System;
using System.Net;
using System.Threading.Tasks;
using SharperCryptoApiAnalysis.Core;

namespace SharperCryptoApiAnalysis.Shell.Interop.Connectivity
{
    public abstract class GitHostServer : IGitHostServer
    {
        protected string RawFilesPathBase => Url.Combine(RawFileDomain, LocalRepoPath, BranchSubPath);

        protected abstract string RawFileDomain { get; }

        protected virtual string BranchSubPath => "master";

        protected string LocalRepoPath { get; private set; }

        protected Uri BaseHttpAddress { get; private set; }

        public void SetBaseAddress(Uri baseAddress)
        {
            BaseHttpAddress = baseAddress;
            LocalRepoPath = baseAddress.LocalPath.TrimStart('/');
        }

        public async Task<bool> IsHostingSharperCryptoApiAnalysis()
        {
            return await Task.FromResult(UrlExists(Constants.ConfigurationFileName, UrlRequestOptions.Default));
        }

        public async Task<string> DownloadString(string resource)
        {
            var webClient = new WebClient();
            webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            var address = Url.Combine(RawFilesPathBase, resource);
            var uri = new Uri(address);
            return await webClient.DownloadStringTaskAsync(uri);
        }

        public bool UrlExists(string resource)
        {
            return UrlExists(resource, UrlRequestOptions.AllowBadRequest | UrlRequestOptions.AllowForbiddenRequest);
        }

        public bool UrlExists(string resource, UrlRequestOptions options)
        {
            return UrlExists(RawFilesPathBase, resource, options);
        }

        private bool UrlExists(string basePath, string resource, UrlRequestOptions options)
        {
            var path = Url.Combine(basePath, resource);
            var request = (HttpWebRequest)WebRequest.Create(path);
            request.Method = "HEAD";
            request.Timeout = 5000;
            try
            {
                request.GetResponse();
                request.Abort();
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response)
                {
                    if (options.HasFlag(UrlRequestOptions.AllowBadRequest) &&
                        response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        return true;
                    }

                    if (options.HasFlag(UrlRequestOptions.AllowForbiddenRequest) &&
                        response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        return true;
                    }
                }

                return false;
            }
            return true;
        }

        public bool IsRunning()
        {
            return UrlExists(BaseHttpAddress.AbsoluteUri, string.Empty, UrlRequestOptions.Default);
        }

        public async Task DownloadFile(string resource, string storagePath)
        {
            if (string.IsNullOrEmpty(resource))
                throw new ArgumentNullException(nameof(resource));

            var webClient = new WebClient();
            webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            var s = Url.Combine(RawFilesPathBase, resource);   
            var uri = new Uri(s);
            await webClient.DownloadFileTaskAsync(uri, storagePath);
        }
    }
}