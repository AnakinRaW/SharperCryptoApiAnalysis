using System.Collections.Generic;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <summary>
    /// Provides all available crypto code generation tasks for Sharper Crypto-API Analysis
    /// </summary>
    public interface ICryptoCodeGenerationTaskProvider
    {
        /// <summary>
        ///  All available tasks.
        /// </summary>
        IReadOnlyCollection<ICryptoCodeGenerationTask> Tasks { get; }
    }
}