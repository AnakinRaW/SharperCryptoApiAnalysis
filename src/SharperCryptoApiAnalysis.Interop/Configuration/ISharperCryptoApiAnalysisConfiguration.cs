using System.Collections.Generic;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Interop.Configuration
{
    /// <summary>
    /// The tools configuration
    /// </summary>
    public interface ISharperCryptoApiAnalysisConfiguration
    {
        /// <summary>
        /// The available extensions.
        /// </summary>
        IEnumerable<ISharperCryptoApiExtensionMetadata> Extensions { get; }

        /// <summary>
        /// The repo address of the configuration.
        /// </summary>
        string RepoAddress { get; }

        /// <summary>
        /// The sync mode.
        /// </summary>
        ConfigSyncMode SyncMode { get; }

        /// <summary>
        /// The analysis severity.
        /// </summary>
        AnalysisSeverity AnalysisSeverity { get; }
    }
}