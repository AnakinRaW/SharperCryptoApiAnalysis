using System;
using System.Net;
using System.Threading.Tasks;

namespace SharperCryptoApiAnalysis.Connectivity.Servers
{
    internal class DefaultServer
    {
        public async Task DownloadFile(Uri absolutePath, string storageFilePath)
        {
            if (absolutePath == null)
                throw new ArgumentNullException(nameof(absolutePath));

            if (!absolutePath.IsAbsoluteUri)
                throw new InvalidOperationException("Uri is not absolute");

            var webClient = new WebClient
            {
                CachePolicy =
                    new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)
            };
            await webClient.DownloadFileTaskAsync(absolutePath, storageFilePath);
        }
    }
}
