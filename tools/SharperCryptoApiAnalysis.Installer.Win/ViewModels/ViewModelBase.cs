using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SharperCryptoApiAnalysis.Installer.Win.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public abstract string Name { get; }

        public abstract FrameworkElement View { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
