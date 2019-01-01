using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Vsix.Ui;

namespace SharperCryptoApiAnalysis.Vsix
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideToolWindow(typeof(SharperCryptoApiAnalysisPane), Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed)]
    [Guid(PackageGuidString)]
    public sealed class SharperCryptoApiAnalysisPanePackage : AsyncPackage
    {
        public const string PackageGuidString = "2817f61b-9415-480c-ba80-57b0f1fdcda0";
    }
}
