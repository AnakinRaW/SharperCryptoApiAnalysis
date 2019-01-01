using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;
using SharperCryptoApiAnalysis.Shell.ViewPattern;

namespace SharperCryptoApiAnalysis.Vsix.Views.ToolWindowPane
{

    public class GenericAnalyzerReportsView : ViewBase<IAnalyzerReportsViewModel, AnalyzerReportsView>
    {
    }

    [ExportViewFor(typeof(IAnalyzerReportsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AnalyzerReportsView
    {
        public AnalyzerReportsView()
        {
            InitializeComponent();
        }
    }
}
