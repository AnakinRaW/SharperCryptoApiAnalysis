using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Settings;

namespace SharperCryptoApiAnalysis.Vsix.Services
{
    public class AnalyzerManager : IAnalyzerManager
    {
        private ISharperCryptoApiAnalysisSettings Settings { get; }

        public ObservableCollection<ISharperCryptoApiAnalysisAnalyzer> Analyzers { get; }

        public ObservableCollection<IAnalysisReport> Reports { get; }

        public ObservableCollection<IAnalysisReport> ReportsHistory { get; }

        public AnalyzerManager(ISharperCryptoApiAnalysisSettings settings)
        {
            Analyzers = new ObservableCollection<ISharperCryptoApiAnalysisAnalyzer>();
            ReportsHistory = new ObservableCollection<IAnalysisReport>();
            Reports = new ObservableCollection<IAnalysisReport>();
            Settings = settings;
        }

        public event EventHandler<IAnalysisReport> Reported;

        public void Report(IAnalysisReport report)
        {
            ReportsHistory.Add(report);
            OnReported(report);
        }

        public void ClearReportHistory()
        {
            ReportsHistory.Clear();
        }

        public void RegisterAnalyzer(ISharperCryptoApiAnalysisAnalyzer analyzer)
        {
            var index = Analyzers.IndexOf(analyzer);
            if (index < 0)
                ThreadHelper.Generic.BeginInvoke(() => Analyzers.Add(analyzer));
            else
                ThreadHelper.Generic.BeginInvoke(() => Analyzers[index] = analyzer);

            if (Settings.MutedAnalyzers.Contains((int) analyzer.AnalyzerId))
                analyzer.IsMuted = true;

            foreach (var report in analyzer.SupportedReports)
            {
                if (report.Equals(AnalysisReport.EmptyReport))
                    continue;
                ThreadHelper.Generic.BeginInvoke(() => Reports.Add(report));
            }
        }

        public bool TryGetReport(string errorCode, out IAnalysisReport report)
        {
            report = AnalysisReport.EmptyReport;

            report = Reports.FirstOrDefault(x => x.Id.Equals(errorCode, StringComparison.InvariantCulture));

            if (report == null)
                return false;
            return true;
        }

        public void PushHistory(IAnalysisReport report)
        {
            if (ReportsHistory.LastOrDefault() == report)
                return;
            ReportsHistory.Add(report);
        }

        protected virtual void OnReported(IAnalysisReport e)
        {
            Reported?.Invoke(this, e);
        }
    }
}