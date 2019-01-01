using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;
using SharperCryptoApiAnalysis.Shell.ViewPattern;

namespace SharperCryptoApiAnalysis.Vsix.Views.ToolWindowPane
{
    public class GenericManageExtensionsView : ViewBase<IManageExtensionsViewModel, ManageExtensionsView>
    {
    }

    [ExportViewFor(typeof(IManageExtensionsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ManageExtensionsView
    {
        public ManageExtensionsView()
        {
            InitializeComponent();
        }
    }
}
