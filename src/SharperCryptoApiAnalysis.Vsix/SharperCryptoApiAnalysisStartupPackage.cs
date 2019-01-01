using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Interop.Settings;
using SharperCryptoApiAnalysis.Vsix.Views.Dialog;
using Task = System.Threading.Tasks.Task;

namespace SharperCryptoApiAnalysis.Vsix
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class SharperCryptoApiAnalysisStartupPackage : AsyncPackage
    {
        public const string PackageGuidString = "427600ef-b61e-4010-8aef-5f2b57420a9e";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            if (!(await GetServiceAsync(typeof(ISharperCryptoApiAnalysisSettings)) is ISharperCryptoApiAnalysisSettings settings))
                return;
            var configurationManager = await GetServiceAsync(typeof(IConfigurationManager)) as IConfigurationManager;
            if (!string.IsNullOrEmpty(settings.ConfigurationRepositoryPath))
            {
                var uri = new Uri(settings.ConfigurationRepositoryPath);
                if (configurationManager?.ConnectionManager != null)
                    await configurationManager.ConnectionManager?.Connect(uri);
            }

            if (settings.ShowStartupWindow && string.IsNullOrEmpty(settings.ConfigurationRepositoryPath))
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync();
                new StartupWindow(settings, this).ShowDialog();
            }

            await JoinableTaskFactory.SwitchToMainThreadAsync();
        }
    }
}
