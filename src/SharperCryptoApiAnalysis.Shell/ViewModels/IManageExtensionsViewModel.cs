using System.Collections.ObjectModel;
using System.Windows.Input;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;

namespace SharperCryptoApiAnalysis.Shell.ViewModels
{
    public interface IManageExtensionsViewModel : IPanePageViewModel
    {
        IConfigurationManager ConfigurationManager { get; }

        ObservableCollection<object> ItemsSource { get; }

        ICommand UpdateCommand { get; }

        ICommand SyncCommand { get; }

        bool SelectAllItems { get; set; }

        bool CheckBoxesEnabled { get; set; }

        string StatusText { get; set; }
    }
}