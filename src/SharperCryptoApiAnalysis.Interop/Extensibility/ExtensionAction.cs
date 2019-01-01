namespace SharperCryptoApiAnalysis.Interop.Extensibility
{
    /// <summary>
    /// Action what to do with an extension
    /// </summary>
    public enum ExtensionAction
    {
        DoNothing,
        /// <summary>
        /// Register the extension to VS
        /// </summary>
        Install,
        /// <summary>
        /// The download the extension form origin and register to VS
        /// </summary>
        DownloadAndInstall,
        /// <summary>
        /// Unregisteres an extension from VS
        /// </summary>
        Uninstall,
        /// <summary>
        /// Unregisteres and extesion from VS and deletes its file
        /// </summary>
        Remove
    }
}