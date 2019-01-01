using System;

namespace SharperCryptoApiAnalysis.Connectivity.Servers
{
    //TODO: Make exportable to use for mef as extension update : [Export(typeof(IGitHostServer))]
    internal sealed class GitHubGitHostServer : GitHostServer
    {
        protected override string RawFileDomain => "https://raw.githubusercontent.com";

        public GitHubGitHostServer(Uri baseAddress) : base(baseAddress)
        {
        }
        
    }
}