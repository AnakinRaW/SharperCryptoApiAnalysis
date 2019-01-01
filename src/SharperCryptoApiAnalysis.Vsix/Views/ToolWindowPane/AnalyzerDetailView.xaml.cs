using System.ComponentModel.Composition;
using System.Windows.Controls;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;
using SharperCryptoApiAnalysis.Shell.ViewPattern;

namespace SharperCryptoApiAnalysis.Vsix.Views.ToolWindowPane
{
    public class GenericAnalyzerDetailView : ViewBase<IAnalyzerDetailViewModel, AnalyzerDetailView>
    {
    }


    [ExportViewFor(typeof(IAnalyzerDetailViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AnalyzerDetailView
    {
        public AnalyzerDetailView()
        {
            InitializeComponent();
        }
    }
}
