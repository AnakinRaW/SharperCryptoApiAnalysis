using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Shell.Interop.Connectivity;

namespace SharperCryptoApiAnalysis.Connectivity.Servers
{
    [GitService("gitlab.com")]
    [Export(typeof(IGitHostServer))]
    internal sealed class GitLabGitHostServer : GitHostServer
    {
        protected override string RawFilesPathBase => Url.Combine(RawFileDomain, LocalRepoPath, "raw", BranchSubPath);

        protected override string RawFileDomain => "https://gitlab.com";
    }
}