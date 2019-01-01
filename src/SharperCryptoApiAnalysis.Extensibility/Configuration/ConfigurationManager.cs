using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SharperCryptoApiAnalysis.Connectivity;
using SharperCryptoApiAnalysis.Connectivity.Exceptions;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.Configuration;
using SharperCryptoApiAnalysis.Interop.Extensibility;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Interop.Settings;

namespace SharperCryptoApiAnalysis.Extensibility.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private bool _isSyncing;
        private ISharperCryptoApiAnalysisConfiguration _configuration;
        public event EventHandler ConfigurationChanged;

        public ISharperCryptoApiAnalysisConfiguration Configuration
        {
            get => _configuration;
            private set
            {
                if (Equals(value, _configuration)) return;
                _configuration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasConfiguration));
            }
        }

        public bool HasConfiguration => Configuration != null;

        public IConnectionManger ConnectionManager { get; }

        public IExtensionManager ExtensionManager { get; }
        public ISharperCryptoApiAnalysisSettings Settings { get; }

        public ConfigurationManager(ISharperCryptoApiAnalysisSettings settings, string configurationSuffix)
        {
            Settings = settings;

            ConnectionManager = new ConnectionManager();
            ExtensionManager = new ExtensionManager(this, configurationSuffix);

            ConnectionManager.Connected += ConnectionManager_Connected;
            ConnectionManager.Disconnected += ConnectionManager_Disconnected;
        }

        public ConfigurationManager(ISharperCryptoApiAnalysisSettings settings) : this(settings, string.Empty)
        {
        }

        public ConfigurationManager()
        {
        }

        public async Task LoadConfiguration()
        {
            EnsureCanLoadConfiguration();  
            var configFileData = await ConnectionManager.GetData(Constants.ConfigurationFileName);
            Configuration = SharperCryptoApiAnalysisConfiguration.FromFile(PublicConfigFile.LoadFromData(configFileData));

            if (!_isSyncing)
                OnConfigurationChanged();
        }

        public async Task Sync()
        {
            if (!HasConfiguration)
                throw new InvalidOperationException();


            _isSyncing = true;

            await LoadConfiguration();
            ExtensionManager.InvalidateAvailableExtensions();

            var updateResult = ExtensionManager.CheckForUpdates(CheckUpdateOption.AllAvailableExtensions);
            var removeResult = ExtensionManager.CheckForUnmanagedExtensions();

            updateResult.Merge(removeResult);

            await ExtensionManager.PerformUpdate(updateResult);
            _isSyncing = false;
        }

        private void ConnectionManager_Disconnected(object sender, EventArgs e)
        {
            Configuration = null;
            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        private async void ConnectionManager_Connected(object sender, EventArgs e)
        {
            await LoadConfiguration();

            var last = Settings.ConfigurationRepositoryPath;
            var current = ConnectionManager.Address;

            if (Configuration.SyncMode == ConfigSyncMode.Hard)
                Settings.Severity = Configuration.AnalysisSeverity;
            else
            {
                if (last != current.AbsoluteUri)
                    Settings.Severity = Configuration.AnalysisSeverity;
            }
            Settings.Save();
        }

        private void EnsureCanLoadConfiguration()
        {
            if (!ConnectionManager.IsConnected)
                throw new NotConnectedException();
        }

        protected virtual void OnConfigurationChanged()
        {
            OnPropertyChanged(nameof(HasConfiguration));
            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
