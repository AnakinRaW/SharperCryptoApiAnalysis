using System;

namespace SharperCryptoApiAnalysis.Interop.Extensibility
{
    /// <summary>
    /// Data entry of an extension action
    /// </summary>
    public struct ExtensionCheckActionDataEntry
    {
        /// <summary>
        /// The extension metadata.
        /// </summary>
        public ISharperCryptoApiExtensionMetadata ExtensionMetadata { get; }

        /// <summary>
        /// The action.
        /// </summary>
        public ExtensionAction Action { get; }

        /// <summary>
        /// The currently installed version.
        /// </summary>
        public Version InstalledVersion { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionCheckActionDataEntry"/> struct.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="action">The action.</param>
        public ExtensionCheckActionDataEntry(ISharperCryptoApiExtensionMetadata metadata, ExtensionAction action)
        {
            ExtensionMetadata = metadata;
            Action = action;
            InstalledVersion = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionCheckActionDataEntry"/> struct.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="action">The action.</param>
        /// <param name="installedVersion">The installed version.</param>
        public ExtensionCheckActionDataEntry(ISharperCryptoApiExtensionMetadata metadata, ExtensionAction action, Version installedVersion)
        {
            ExtensionMetadata = metadata;
            Action = action;
            InstalledVersion = installedVersion;
        }

        public override string ToString()
        {
            return ExtensionMetadata.Name;
        }
    }
}