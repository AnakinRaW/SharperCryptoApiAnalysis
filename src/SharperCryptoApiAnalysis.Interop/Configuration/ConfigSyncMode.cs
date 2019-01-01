namespace SharperCryptoApiAnalysis.Interop.Configuration
{
    /// <summary>
    /// The configuration mode used for git based configurations
    /// </summary>
    public enum ConfigSyncMode
    {
        /// <summary>
        /// Undefined mode
        /// </summary>
        Undefined,
        /// <summary>
        /// Allows manual configuration of the tool
        /// </summary>
        Soft,
        /// <summary>
        /// Disallows manual configuration of the tool
        /// </summary>
        Hard
    }
}