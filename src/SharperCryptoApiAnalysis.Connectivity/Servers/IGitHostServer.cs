using System;
using System.Threading.Tasks;

namespace SharperCryptoApiAnalysis.Connectivity.Servers
{
    public interface IGitHostServer
    {
        void SetBaseAddress(Uri address);

        Task<bool> IsHostingSharperCryptoApiAnalysis();

        Task<string> DownloadString(string resource);

        bool UrlExists(string resource);

        bool UrlExists(string resource, UrlRequestOptions options);

        bool IsRunning();

        Task DownloadFile(string resource, string storagePath);    
    }
}