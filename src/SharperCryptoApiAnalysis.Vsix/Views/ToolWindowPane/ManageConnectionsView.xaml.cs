using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;

namespace SharperCryptoApiAnalysis.Vsix.Views.ToolWindowPane
{
    public class GenericManageConnectionsView : ViewBase<IManageConnectionsViewModel, ManageConnectionsView>
    {
    }

    [ExportViewFor(typeof(IManageConnectionsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ManageConnectionsView
    {
        public ManageConnectionsView()
        {
            InitializeComponent();
        }
    }
}
