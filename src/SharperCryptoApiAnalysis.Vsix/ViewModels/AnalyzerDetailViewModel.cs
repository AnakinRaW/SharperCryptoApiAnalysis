using System;
using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Interop.Settings;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels
{
    [Export(typeof(IAnalyzerDetailViewModel))]
    public class AnalyzerDetailViewModel : PanePageViewModelBase, IAnalyzerDetailViewModel
    {
        private readonly ISharperCryptoAnalysisServiceProvider _serviceProvider;
        private IAnalysisReport _activeReport;
        public override string Title => "Analyzer Information";

        public IAnalyzerManager AnalyzerManager { get; }

        public IAnalysisReport ActiveReport
        {
            get => _activeReport;
            set
            {
                if (Equals(value, _activeReport)) return;
                _activeReport = value;
                OnPropertyChanged();
                OnActiveReportChanged(value);
            }
        }

        private void OnActiveReportChanged(IAnalysisReport report)
        {
            AnalyzerManager.PushHistory(report);
        }

        [ImportingConstructor]
        public AnalyzerDetailViewModel(ISharperCryptoAnalysisServiceProvider serviceProvider, ISettingsProvider configuration)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            AnalyzerManager = _serviceProvider.GetService<IAnalyzerManager>();

            AnalyzerManager.Reported += OnReported;
        }

        private void OnReported(object sender, IAnalysisReport e)
        {
            _activeReport = e;
            OnPropertyChanged(nameof(ActiveReport));
        }
    }
}
