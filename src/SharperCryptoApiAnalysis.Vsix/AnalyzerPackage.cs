using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Runtime.InteropServices;
using CommonServiceLocator;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Settings;
using SharperCryptoApiAnalysis.Vsix.Services;

namespace SharperCryptoApiAnalysis.Vsix
{
    [Guid(AnalyzerPackageGuidString)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // We need to make sure the Mef Provider gets as early initialized as possible. Shell activation for its own is too slow because VS allows loading projects even
    // when the Shell is not yet initialized.
    [ProvideAutoLoad(UIContextGuids.PackageActivation)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    public class AnalyzerPackage : Package
    {
        public const string AnalyzerPackageGuidString = "353123DF-1627-4505-A3E7-DCA193430348";

        protected override void Initialize()
        {
            base.Initialize();
            var analyzerManager = GetService(typeof(IAnalyzerManager)) as IAnalyzerManager;
            var provider = SharperCryptoAnalysisServiceProvider.Instance;
            //Create Singletone instance here to prevent some odd behaviour not finding the contract at some other time.
            provider.GetService<ISettingsProvider>();
            var container = new CompositionContainer(CompositionOptions.Default, provider.ExportProvider);
            container.ComposeExportedValue(analyzerManager);
            var serviceLocator = new MefLocatorProvider(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }
    }
}