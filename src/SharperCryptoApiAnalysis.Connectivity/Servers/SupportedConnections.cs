using System;

namespace SharperCryptoApiAnalysis.Connectivity.Servers
{
    //TODO: Make this a mef component that imports all IGitHostServer instances
    internal static class SupportedConnections
    {

        public static bool IsConnectionHostSupported(string hostAddress, out SupportedHosts supportedHost)
        {
            switch (hostAddress)
            {
                case "github.com":
                    supportedHost = SupportedHosts.Github;
                    return true;
                default:
                    supportedHost = SupportedHosts.None;
                    return false;
            }
        }

        public static IGitHostServer CreateHostServer(SupportedHosts host, Uri baseAddress)
        {
            switch (host)
            {
                case SupportedHosts.Github:
                    return new GitHubGitHostServer(baseAddress);
                default:
                    throw new ArgumentOutOfRangeException(nameof(host), host, null);
            }
        }
    }
}