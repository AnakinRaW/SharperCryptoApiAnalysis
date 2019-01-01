using System.Threading.Tasks;
using SharperCryptoApiAnalysis.Shell.ViewModels;

namespace SharperCryptoApiAnalysis.Shell
{
    /// <summary>
    /// Interface for accessing the ToolWindowPane
    /// </summary>
    public interface IToolWindowManager
    {
        /// <summary>
        /// Ensure that the pane is created and visible.
        /// </summary>
        /// <returns>The view model for the pane.</returns>
        Task<ISharperCryptoApiAnalysisPaneViewModel> ShowToolPane();
    }
}
