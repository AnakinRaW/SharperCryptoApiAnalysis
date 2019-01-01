using System.Windows.Input;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;

namespace SharperCryptoApiAnalysis.Shell.ViewModels
{
    public interface IManageConnectionsViewModel : IPanePageViewModel
    {
        IConfigurationManager ConfigurationManager { get; }

        ICommand ConnectCommand { get; }
        ICommand SyncModeInfoCommand { get; }
        ICommand DisconnectCommand { get; }

        string SyncModeText { get; }

        string RepositoryPath { get; set; }
    }
}