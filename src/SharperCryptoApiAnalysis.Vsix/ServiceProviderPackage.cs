using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SharperCryptoApiAnalysis.Extensibility.Configuration;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Interop.Settings;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.ViewModels;
using SharperCryptoApiAnalysis.Vsix.Services;
using SharperCryptoApiAnalysis.Vsix.Settings;
using SharperCryptoApiAnalysis.Vsix.Ui;
using Task = System.Threading.Tasks.Task;

namespace SharperCryptoApiAnalysis.Vsix
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideService(typeof(ISharperCryptoAnalysisServiceProvider), IsAsyncQueryable = true)]
    [ProvideService(typeof(IToolWindowManager), IsAsyncQueryable = true)]
    [ProvideService(typeof(IConfigurationManager), IsAsyncQueryable = true)]
    [ProvideService(typeof(ISharperCryptoApiAnalysisSettings), IsAsyncQueryable = true)]
    [ProvideService(typeof(IAnalyzerManager), IsAsyncQueryable = true)]
    [Guid(ServiceProviderPackageId)]
    public sealed class ServiceProviderPackage : AsyncPackage, IServiceProviderPackage, IToolWindowManager
    {
        public const string ServiceProviderPackageId = "9D466EFE-AC1F-4D73-BDDB-9D841522935F";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {

#if DEBUG
            await CheckBindingPathsAsync();
#endif

            AddService(typeof(ISharperCryptoAnalysisServiceProvider), CreateServiceAsync, true);
            AddService(typeof(IToolWindowManager), CreateServiceAsync, true);
            AddService(typeof(IExtensionManager), CreateServiceAsync, true);
            AddService(typeof(IConnectionManger), CreateServiceAsync, true);
            AddService(typeof(IConfigurationManager), CreateServiceAsync, true);
            AddService(typeof(ISharperCryptoApiAnalysisSettings), CreateServiceAsync, true);  
            AddService(typeof(IAnalyzerManager), CreateServiceAsync, true);  
        }

        private async Task CheckBindingPathsAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            BindingPathHelper.CheckBindingPaths(GetType().Assembly, this);
        }

        private async Task<object> CreateServiceAsync(IAsyncServiceContainer container, CancellationToken cancellationToken, Type serviceType)
        {
            if (serviceType == null)
                return null;

            if (container != this)
                return null;

            if (serviceType == typeof(ISharperCryptoAnalysisServiceProvider))
            {
                var result = new SharperCryptoAnalysisServiceProvider(this, this);
                await result.InitializeAsync();
                return result;
            }
            if (serviceType == typeof(IToolWindowManager))
                return this;
            if (serviceType == typeof(IConfigurationManager))
            {
                var settings = await GetServiceAsync(typeof(ISharperCryptoApiAnalysisSettings)) as ISharperCryptoApiAnalysisSettings;
                return new ConfigurationManager(settings);
            }

            if (serviceType == typeof(IAnalyzerManager))
            {
                var settings = await GetServiceAsync(typeof(ISharperCryptoApiAnalysisSettings)) as ISharperCryptoApiAnalysisSettings;
                return new AnalyzerManager(settings);
            }
            if (serviceType == typeof(ISharperCryptoApiAnalysisSettings))
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var dte = new ServiceProvider(VisualStudio.Integration.Services.Dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
                return new SharperCryptoApiAnalysisSettings(dte);
            }

            var sp = await GetServiceAsync(typeof(ISharperCryptoAnalysisServiceProvider)) as ISharperCryptoAnalysisServiceProvider;
            return sp.TryGetService(serviceType);
        }

        public async Task<ISharperCryptoApiAnalysisPaneViewModel> ShowToolPane()
        {
            var pane = ShowToolWindow(new Guid(SharperCryptoApiAnalysisPane.SharperCryptoAnalysisPaneGuid));
            if (pane == null)
                return null;
            if (pane.Frame is IVsWindowFrame frame)
                ErrorHandler.Failed(frame.Show());
            var scaPane = (SharperCryptoApiAnalysisPane) pane;
            return await scaPane.GetViewModelAsync();
        }

        private static ToolWindowPane ShowToolWindow(Guid windowGuid)
        {
            if (ErrorHandler.Failed(VisualStudio.Integration.Services.VsUiShell.FindToolWindow((uint)__VSCREATETOOLWIN.CTW_fForceCreate,
                ref windowGuid, out var frame)))
                return null;
            if (ErrorHandler.Failed(frame.Show()))
                return null;

            if (ErrorHandler.Failed(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out var docView)))
                return null;
            return docView as SharperCryptoApiAnalysisPane;
        }
    }
}
