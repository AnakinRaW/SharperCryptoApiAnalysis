using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;

namespace SharperCryptoApiAnalysis.Vsix.Views.ToolWindowPane
{
    public class GenericGitHubPaneView : ViewBase<ISharperCryptoApiAnalysisPaneViewModel, SharperCryptoAnalysisPaneView>
    {
    }

    [ExportViewFor(typeof(ISharperCryptoApiAnalysisPaneViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SharperCryptoAnalysisPaneView
    {
        [ImportingConstructor]
        public SharperCryptoAnalysisPaneView()
        {
            InitializeComponent();
        }
    }
}