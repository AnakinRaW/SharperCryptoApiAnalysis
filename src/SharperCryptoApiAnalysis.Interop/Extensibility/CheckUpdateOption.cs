namespace SharperCryptoApiAnalysis.Interop.Extensibility
{
    /// <summary>
    /// Option how the extension manager shall handle updates
    /// </summary>
    public enum CheckUpdateOption
    {
        /// <summary>
        /// Update all available extensions
        /// </summary>
        AllAvailableExtensions,
        /// <summary>
        /// Update only installed extensions
        /// </summary>
        OnlyInstalledExtensions
    }
}