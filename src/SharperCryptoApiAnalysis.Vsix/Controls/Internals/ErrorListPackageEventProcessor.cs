using System;
using CommonServiceLocator;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.ViewModels;
using SharperCryptoApiAnalysis.Vsix.Commands;

namespace SharperCryptoApiAnalysis.Vsix.Controls.Internals
{
    internal class ErrorListPackageEventProcessor : TableControlEventProcessorBase
    {
        private readonly ISharperCryptoAnalysisServiceProvider _serviceProvider;

        private IAnalyzerManager _analyzerManager;
        private readonly IAnalyzerDetailViewModel _viewModel;
        private readonly ISharperCryptoApiAnalysisPaneViewModel _toolWindow;

        protected IAnalyzerManager AnalyzerManager
        {
            get
            {
                if (_analyzerManager == null)
                {
                    if (ServiceLocator.IsLocationProviderSet)
                        _analyzerManager = ServiceLocator.Current.GetInstance<IAnalyzerManager>();
                }
                return _analyzerManager;
            }
        }

        public ErrorListPackageEventProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider as ISharperCryptoAnalysisServiceProvider;
            if (_serviceProvider == null)
                throw new ArgumentException("Wrong argument type");

            var toolManager = _serviceProvider.GetService(typeof(IToolWindowManager)) as IToolWindowManager;
            _toolWindow = toolManager?.ShowToolPane().Result;
            _viewModel = _serviceProvider.ExportProvider.GetExportedValue<IAnalyzerDetailViewModel>();      
        }

        public override void PreprocessNavigateToHelp(ITableEntryHandle entryHandle, TableEntryEventArgs e)
        {
            if (!entryHandle.TryGetValue<string>("errorcode", out var code))
                return;

            if (!code.StartsWith(DiagnosticPrefix.Prefix, StringComparison.InvariantCulture))
                return;

            if (!AnalyzerManager.TryGetReport(code, out var report))
                return;

            AnalysisCommands.ShowReportCommand.Execute(report);
            e.Handled = true;
        }
    }
}