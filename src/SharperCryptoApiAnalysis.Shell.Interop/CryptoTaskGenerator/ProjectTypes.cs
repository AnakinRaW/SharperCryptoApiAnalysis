using System;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <summary>
    /// Project types supported by Sharper Crypto-API Analysis
    /// </summary>
    [Flags]
    public enum CompatibleProjectTypes
    {
        /// <summary>
        /// .NET Core
        /// </summary>
        Core = 1,
        /// <summary>
        /// .NET Framework
        /// </summary>
        Framework = 2,
        /// <summary>
        /// .NET Standard 1.0
        /// </summary>
        StandardV1 = 4,
        /// <summary>
        /// .NET Standard 2.0
        /// </summary>
        StandardV2 = 8,
    }
}