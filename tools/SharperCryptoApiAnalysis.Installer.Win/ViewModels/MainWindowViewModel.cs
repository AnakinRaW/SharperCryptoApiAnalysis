using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SharperCryptoApiAnalysis.Installer.Win.ViewModels
{
    internal sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private ViewModelBase _content;

        public ViewModelBase Content
        {
            get => _content;
            set
            {
                if (Equals(value, _content)) return;
                _content = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            Content = new RepositoryConfiguratorViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
