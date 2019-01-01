using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Input.Command;
using SharperCryptoApiAnalysis.Interop.Configuration;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Interop.Settings;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels
{
    //TODO: Add IsBusy, etc. to code
    [Export(typeof(IManageConnectionsViewModel))]
    public class ManageConnectionsViewModel : PanePageViewModelBase, IManageConnectionsViewModel
    {
        private string _repositoryPath;
        private ISharperCryptoApiAnalysisSettings _settings;
        public override string Title => "Manage Connection";

        public IConfigurationManager ConfigurationManager { get; }

        public ICommand ConnectCommand => new Command(Connect);
        public ICommand SyncModeInfoCommand => new Command(ShowSyncModeInfo);
        public ICommand DisconnectCommand => new Command(Disconnect);

        public string SyncModeText { get; private set; }

        public string RepositoryPath
        {
            get => _repositoryPath;
            set
            {
                if (value == _repositoryPath) return;
                _repositoryPath = value;
                OnPropertyChanged();
            }
        }

        [ImportingConstructor]
        public ManageConnectionsViewModel(ISettingsProvider settingsProvider)
        {
            _settings = settingsProvider.Settings;

            ConfigurationManager =
                VisualStudio.Integration.Services.SharperCryptoAnalysisServiceProvider.GetService(
                    typeof(IConfigurationManager)) as IConfigurationManager ?? throw new InvalidOperationException();

            ConfigurationManager.ConfigurationChanged += OnConfigurationChanged;

            ConfigurationManager.ConnectionManager.Connected += ConnectionManager_Connected;
        }

        private void ConnectionManager_Connected(object sender, EventArgs e)
        {
            RepositoryPath = ConfigurationManager.ConnectionManager.Address.AbsoluteUri;
        }

        private void OnConfigurationChanged(object sender, EventArgs e)
        {
            if (!ConfigurationManager.HasConfiguration)
                SyncModeText = "";
            else
                SyncModeText = ConfigurationManager.Configuration.SyncMode == ConfigSyncMode.Hard ? "External Management" : "Manual Management";

            OnPropertyChanged(nameof(SyncModeText));
        }

        private void ShowSyncModeInfo()
        {
            string message = string.Empty;
            if (ConfigurationManager.Configuration.SyncMode == ConfigSyncMode.Hard)
                message = "Management mode: External Management\r\n\r\n" +
                          "With this configuration all extensions and settings are managed and configured from the settings provided by the specified repository." +
                          "You can not install custom extensions.";
            else
                message = "Management mode: Manual Management\r\n\r\n" +
                          "With this configuration extensions are managed and configured from the settings provided by the specified repository." +
                          "You may install additional custom extensions.";

            MessageBox.Show(message, "Sharper Crypto-API Analysis", MessageBoxButton.OK, MessageBoxImage.Information,
                MessageBoxResult.OK);
        }

        private void Disconnect()
        {
            ConfigurationManager.ConnectionManager.Disconnect();
        }

        private async void Connect()
        {
            try
            {
                await ConfigurationManager.ConnectionManager.Connect(new Uri(RepositoryPath));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            StoreSettings();
        }

        private void StoreSettings()
        {
            _settings.ConfigurationRepositoryPath = ConfigurationManager.ConnectionManager.Address.AbsoluteUri;
            _settings.Save();
        }

    }
}