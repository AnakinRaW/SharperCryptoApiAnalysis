using System.ComponentModel.Composition;

namespace SharperCryptoApiAnalysis.Connectivity.Servers
{
    [GitService("github.com")]
    [Export(typeof(IGitHostServer))]
    internal sealed class GitHubGitHostServer : GitHostServer
    {
        protected override string RawFileDomain => "https://raw.githubusercontent.com";       
    }
}