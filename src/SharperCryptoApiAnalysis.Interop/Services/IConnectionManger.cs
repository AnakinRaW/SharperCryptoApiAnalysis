using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SharperCryptoApiAnalysis.Interop.Services
{
    public interface IConnectionManger : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a connection was established.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// Occurs when a connection was closed.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        /// The address of the connected server.
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// Indicating whether a connection is established.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if a connection is established; otherwise, <see langword="false"/>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to a given address.
        /// </summary>
        /// <param name="remoteAddress">The address.</param>
        /// <returns>The Task</returns>
        Task Connect(Uri remoteAddress);

        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <param name="resourceName">Relative path of the resource.</param>
        /// <param name="storageFilePath">The absolute storage file path.</param>
        /// <returns>The Task</returns>
        Task DownloadFile(string resourceName, string storageFilePath);

        /// <summary>
        /// Downloads an external file.
        /// </summary>
        /// <param name="absolutePath">The absolute file location.</param>
        /// <param name="storageFilePath">The absolute storage file path.</param>
        /// <returns>The Task</returns>
        Task DownloadExternalFile(Uri absolutePath, string storageFilePath);

        /// <summary>
        /// Downloads a string.
        /// </summary>
        /// <param name="resourceName">Relative resource path.</param>
        /// <returns>The Task</returns>
        Task<string> GetData(string resourceName);

        void Disconnect();
    }
}