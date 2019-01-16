using System.ComponentModel;
using System.Reflection.Metadata;
using System.Windows.Input;
using ModernApplicationFramework.Input.Command;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.Configuration;
using SharperCryptoApiAnalysis.Interop.Extensibility;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Vsix.ViewModels.Extension;
using SharperCryptoApiAnalysis.Vsix.Views.ToolWindowPane;

namespace SharperCryptoApiAnalysis.Vsix.Commands
{
    internal static class ExtensionCommands
    {
        private static IConfigurationManager _configurationManager;

        private static IConfigurationManager ConfigurationManager =>
            _configurationManager ?? (_configurationManager = Services.SharperCryptoAnalysisServiceProvider
                .Instance.GetService<IConfigurationManager>());

        public static ICommand UninstallExtensionCommand { get; } = new Command(UninstallExtension, CanUninstallExtension);

        public static ICommand InstallExtensionCommand { get; } = new Command(InstallExtension, CanInstallExtension);

        private static bool CanInstallExtension(object arg)
        {

#if DEBUG
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(ManageExtensionsView)).DefaultValue))
                return true;
#endif
            if (!ConfigurationManager.HasConfiguration || ConfigurationManager.Configuration.SyncMode == ConfigSyncMode.Hard)
                return false;
            return true;
        }


        private static async void InstallExtension(object obj)
        {
            if (!(obj is IExtensionItemListViewModel viewModel))
                return;

            if (string.IsNullOrEmpty(viewModel.ExtensionMetadata.Source))
                return;


            var metaData = viewModel.ExtensionMetadata;

            await ConfigurationManager.ExtensionManager.PerformUpdate(
                new ExtensionCheckActionData(
                    new ExtensionCheckActionDataEntry(metaData, ExtensionAction.DownloadAndInstall)));
        }


        private static bool CanUninstallExtension(object arg)
        {

#if DEBUG
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(ManageExtensionsView)).DefaultValue))
                return true;
#endif

            if (arg is ExtensionItemListViewModel listViewModel &&
                listViewModel.ExtensionMetadata.Name.Equals(Constants.DefaultAnalyzerAssemblyName))
                return false;

            if (!ConfigurationManager.HasConfiguration || ConfigurationManager.Configuration.SyncMode == ConfigSyncMode.Hard)
                return false;

            return true;
        }

        private static async void UninstallExtension(object obj)
        {
            if (!(obj is IExtensionItemListViewModel viewModel))
                return;

            var metaData = viewModel.ExtensionMetadata;

            await ConfigurationManager.ExtensionManager.PerformUpdate(
                new ExtensionCheckActionData(
                    new ExtensionCheckActionDataEntry(metaData, ExtensionAction.Uninstall)));
        }
    }
}
