namespace SharperCryptoApiAnalysis.Interop.Settings
{
    /// <summary>
    /// Provides an instance of the settings for Sharper Crypto-API Analysis
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// The settings.
        /// </summary>
        ISharperCryptoApiAnalysisSettings Settings { get; }
    }
}