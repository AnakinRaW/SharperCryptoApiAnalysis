using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels.SampleData
{
    internal class ReportViewSampleData : ViewModelBase
    {
        private IAnalysisReport _activeReport;

        public ReportViewSampleData()
        {
            ActiveReport = new SmapleReport();
        }

        public IAnalysisReport ActiveReport
        {
            get => _activeReport;
            set
            {
                if (Equals(value, _activeReport)) return;
                _activeReport = value;
                OnPropertyChanged();
            }
        }
    }

    internal sealed class SmapleReport : IAnalysisReport
    {
        public object AdditionalContent => new Border {Background = Brushes.Red, Height = 100};
        public string Category => "Test Category";
        public string Description => "Description";
        public string Id => "Test id";
        public Uri MoreDetailsUrl => new Uri("http://google.de");
        public IEnumerable<NamedLink> RelatedLinks => new List<NamedLink>{new NamedLink("Google", new Uri("http://google.de")), new NamedLink("Google", new Uri("http://google.de"))};
        public string Severity => "High";
        public Exploitability Exploitability => Exploitability.High;
        public string SolutionRemarks => "Learn how to code";
        public string Summary => "Summary";
        public SecurityGoals ExposedSecurityGoals => SecurityGoals.Authenticity | SecurityGoals.Integrity;
    }

}
