using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SharperCryptoApiAnalysis.Shell.Commands;
using SharperCryptoApiAnalysis.Shell.Interop.Commands;
using SharperCryptoApiAnalysis.VisualStudio.Integration;
using SharperCryptoApiAnalysis.Vsix.Settings;

namespace SharperCryptoApiAnalysis.Vsix
{
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.SharperCryptoApiAnalysisPackageString)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [PackageRegistration(UseManagedResourcesOnly = true)] 
    [ComVisible(true)]
    [ProvideAutoLoad(UIContextGuids.PackageActivation)]
    [ProvideUIContextRule(UIContextGuids.PackageActivation,
        "SharperCryptoApiAnalysisPackageActivation",
        "(HasCSProj)",
        new[] { "HasCSProj"},
        new[] { "SolutionHasProjectCapability:CSharp" }
    )]
    [ProvideOptionPage(typeof(GeneralOptionsDialogPage), "Sharper Crypto-API Analysis", "General", 901, 902, false, 903)]
    [ProvideOptionPage(typeof(AnalyzersOptionsDialogPage), "Sharper Crypto-API Analysis", "Analyzers", 901, 902, false, 903)]
    public class SharperCryptoApiAnalysisPackage :  Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            InitializeMenus();
        }

        protected void InitializeMenus()
        {
            IVsCommandBase[] commands;
            if (ProcessHelper.IsRunningInsideVisualStudio())
            {
                var componentModel = (IComponentModel)GetService(typeof(SComponentModel));
                var exports = componentModel.DefaultExportProvider;

                commands = new IVsCommandBase[]
                {
                    exports.GetExportedValue<IAboutCommand>(),
                    exports.GetExportedValue<IShowSharperCryptoAnalysisPaneCommand>(),
                    exports.GetExportedValue<IAddCryptoTaskCommand>(),
                };
            }
            else
            {
                commands = new IVsCommandBase[0];
                VsShellUtilities.ShowMessageBox(this, "This extension only is available in Visual Studio", null,
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }

            var menuService = (IMenuCommandService) GetService(typeof(IMenuCommandService));
            menuService?.AddCommands(commands);
        }
    }
}
