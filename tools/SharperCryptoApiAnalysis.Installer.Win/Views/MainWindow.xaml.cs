using SharperCryptoApiAnalysis.Installer.Win.ViewModels;

namespace SharperCryptoApiAnalysis.Installer.Win.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
