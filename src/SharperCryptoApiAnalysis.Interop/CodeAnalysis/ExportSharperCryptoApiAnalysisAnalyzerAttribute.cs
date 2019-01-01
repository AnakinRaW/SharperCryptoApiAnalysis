using System.Collections.Immutable;
using System.ComponentModel;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis
{
    /// <inheritdoc />
    /// <summary>
    /// Analyzer for Sharper Crypto-API Analysis
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public interface ISharperCryptoApiAnalysisAnalyzer : INotifyPropertyChanged
    {
        /// <summary>
        /// The name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The analyzer identifier.
        /// </summary>
        uint AnalyzerId { get; }

        /// <summary>
        /// Indicating whether the analyzer is muted.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance is muted; otherwise, <see langword="false"/>.
        /// </value>
        bool IsMuted { get; set; }

        /// <summary>
        /// Supported reports.
        /// </summary>
        ImmutableArray<IAnalysisReport> SupportedReports { get; }
    }
}
