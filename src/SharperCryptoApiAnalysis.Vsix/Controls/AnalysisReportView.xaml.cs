using System.Windows.Navigation;
using Microsoft.VisualStudio.Shell;

namespace SharperCryptoApiAnalysis.Vsix.Controls
{
    public partial class AnalysisReportView
    {
        public AnalysisReportView()
        {
            InitializeComponent();
        }

        private void OnNavigateRequested(object sender, RequestNavigateEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Uri.ToString()))
                return;
            VsShellUtilities.OpenSystemBrowser(e.Uri.ToString());
        }
    }
}
