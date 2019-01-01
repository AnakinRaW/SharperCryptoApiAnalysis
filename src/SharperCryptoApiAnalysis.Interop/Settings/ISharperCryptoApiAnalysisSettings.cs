using System.Collections.Generic;
using System.ComponentModel;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.Interop.Settings
{
    /// <inheritdoc />
    /// <summary>
    /// The settings of Sharper Crypto-API Analysis
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public interface ISharperCryptoApiAnalysisSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Indicating whether the startup window shall be displayed.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the window should be shown; otherwise, <see langword="false"/>.
        /// </value>
        bool ShowStartupWindow { get; set; }

        /// <summary>
        /// The path to the configuration repository.
        /// </summary>
        string ConfigurationRepositoryPath { get; set; }

        /// <summary>
        /// The code analysis severity.
        /// </summary>
        AnalysisSeverity Severity { get; set; }

        /// <summary>
        /// List of muted analyzers.
        /// </summary>
        HashSet<int> MutedAnalyzers { get; }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        void Save();
    }
}