using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Interop.Services
{
    /// <inheritdoc />
    /// <summary>
    /// An instance that manages extensions 
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public interface IExtensionManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the manager has completed an update task.
        /// </summary>
        event EventHandler UpdateCompleted;

        /// <summary>
        /// The install path of extensions.
        /// </summary>
        string InstallPath { get; }

        /// <summary>
        /// Number of currently installed extensions.
        /// </summary>
        int ExtensionsCount { get; }

        /// <summary>
        /// Indicating whether this instance can currently check for updates.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance can check updates; otherwise, <see langword="false"/>.
        /// </value>
        bool CanCheckUpdates { get; }

        /// <summary>
        /// A list of locally available extensions
        /// </summary>
        IReadOnlyCollection<ISharperCryptoApiExtensionMetadata> LocalExtensions { get; }

        /// <summary>
        /// List of installed extensions.
        /// </summary>
        ReadOnlyObservableCollection<ISharperCryptoApiExtensionMetadata> InstalledExtensions { get; }

        /// <summary>
        /// Determines whether an extension is installed.
        /// </summary>
        /// <param name="extension">The extension metadata.</param>
        /// <param name="installedVersion">The installed version.</param>
        /// <returns>
        ///   <see langword="true"/> if the extension is installed; otherwise, <see langword="false"/>.
        /// </returns>
        bool IsExtensionInstalled(ISharperCryptoApiExtensionMetadata extension, out Version installedVersion);

        /// <summary>
        /// Checks for updates.
        /// </summary>
        /// <param name="option">Update search options.</param>
        /// <returns>The result of the update search</returns>
        ExtensionCheckActionData CheckForUpdates(CheckUpdateOption option);

        /// <summary>
        /// Performs the update of extensions.
        /// </summary>
        /// <param name="updateData">The update data.</param>
        /// <returns>The update task</returns>
        Task PerformUpdate(ExtensionCheckActionData updateData);

        /// <summary>
        /// Checks for installed extensions that are not listed in the current configuration repository.
        /// </summary>
        /// <returns>The result of the search</returns>
        ExtensionCheckActionData CheckForUnmanagedExtensions();

        /// <summary>
        /// Invalidates the installed extensions cache.
        /// </summary>
        void InvalidateAvailableExtensions();

        /// <summary>
        /// Gets the latest available version of an extensions.
        /// </summary>
        /// <param name="metadata">The extension.</param>
        /// <returns>The latest version</returns>
        Version GetLatestAvailableVersion(ISharperCryptoApiExtensionMetadata metadata);
    }
}