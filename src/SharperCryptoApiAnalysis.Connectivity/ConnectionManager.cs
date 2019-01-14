using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Connectivity.Exceptions;
using SharperCryptoApiAnalysis.Connectivity.Servers;
using SharperCryptoApiAnalysis.Interop.Services;
using Task = System.Threading.Tasks.Task;

namespace SharperCryptoApiAnalysis.Connectivity
{
    public class ConnectionManager : IConnectionManger
    {
        private bool _isConnected;
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        internal IGitHostServer Server { get; private set; }

        internal DefaultServer DefaultServer { get; }

        public Uri Address { get; private set; }

        private GitServiceRegistry GitServiceRegistry { get; }

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (value == _isConnected) return;
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public ConnectionManager()
        {
            DefaultServer = new DefaultServer();
            var provider = (ISharperCryptoAnalysisServiceProvider) Package.GetGlobalService(typeof(ISharperCryptoAnalysisServiceProvider));
            GitServiceRegistry = provider.ExportProvider.GetExportedValueOrDefault<GitServiceRegistry>();
        }

        public async Task Connect(Uri remoteAddress)
        {
            if (IsConnected)
                Disconnect();
    
            var hostName = remoteAddress.Host;

            var hostServer = GitServiceRegistry.GetServiceFromHostUrl(hostName);
            if (hostServer == null)
                throw new ConnectionHostNotSupportedException();

            hostServer.SetBaseAddress(remoteAddress);

            if (!hostServer.IsRunning())
                throw new HostRepositoryNotFoundException();

            if (!await hostServer.IsHostingSharperCryptoApiAnalysis())
                throw new RepositoryNotHostingSharperCryptoApiAnalysisException();

            Address = remoteAddress;
            Server = hostServer;
            IsConnected = true;

            Connected?.Invoke(this, EventArgs.Empty);
        }

        public void Disconnect()
        {
            Server = null;
            IsConnected = false;
            Address = null;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public async Task DownloadFile(string resourceName, string storageFilePath)
        {
            if (!IsConnected)
                throw new NotConnectedException();

            if (string.IsNullOrEmpty(storageFilePath))
                throw new ArgumentNullException(nameof(storageFilePath));

            if (!Directory.Exists(Path.GetDirectoryName(storageFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(storageFilePath));

            await Server.DownloadFile(resourceName, storageFilePath);
        }

        public async Task DownloadExternalFile(Uri absolutePath, string storageFilePath)
        {
            if (string.IsNullOrEmpty(storageFilePath))
                throw new ArgumentNullException(nameof(storageFilePath));

            if (!Directory.Exists(Path.GetDirectoryName(storageFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(storageFilePath));

            await DefaultServer.DownloadFile(absolutePath, storageFilePath);
        }

        public async Task<string> GetData(string resourceName)
        {
            if (!IsConnected)
                throw new NotConnectedException();
            return await Server.DownloadString(resourceName);
        }

        protected virtual void OnConnectionChanged()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
