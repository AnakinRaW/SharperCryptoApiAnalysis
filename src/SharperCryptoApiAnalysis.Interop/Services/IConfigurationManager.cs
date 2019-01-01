using System;
using System.ComponentModel;
using System.Threading.Tasks;
using SharperCryptoApiAnalysis.Interop.Configuration;

namespace SharperCryptoApiAnalysis.Interop.Services
{
    /// <inheritdoc />
    /// <summary>
    /// An instance that managed a git configuration
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public interface IConfigurationManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the current configuration was changed.
        /// </summary>
        event EventHandler ConfigurationChanged;

        /// <summary>
        /// The configuration.
        /// </summary>
        ISharperCryptoApiAnalysisConfiguration Configuration { get; }

        /// <summary>
        /// The connection manager.
        /// </summary>
        IConnectionManger ConnectionManager { get; }

        /// <summary>
        /// The extension manager.
        /// </summary>
        IExtensionManager ExtensionManager { get; }

        /// <summary>
        /// Indicating whether a configuration is present.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if a configuration is present; otherwise, <see langword="false"/>.
        /// </value>
        bool HasConfiguration { get; }

        /// <summary>
        /// Loads the configuration from the current connection.
        /// </summary>
        /// <returns>The task</returns>
        Task LoadConfiguration();

        /// <summary>
        /// Synchronizes extensions and settings with the configuration.
        /// </summary>
        /// <returns>The task</returns>
        Task Sync();
    }
}