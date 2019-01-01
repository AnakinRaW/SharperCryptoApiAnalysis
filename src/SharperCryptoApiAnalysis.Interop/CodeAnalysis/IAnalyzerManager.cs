using System;
using System.Collections.ObjectModel;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis
{
    /// <summary>
    /// An instance that manages analyzers in Sharper Crypto-API Analysis
    /// </summary>
    public interface IAnalyzerManager
    {
        /// <summary>
        /// Occurs when a report was issued.
        /// </summary>
        event EventHandler<IAnalysisReport> Reported;

        //TODO Make Readonly
        /// <summary>
        /// The registered reports.
        /// </summary>
        ObservableCollection<IAnalysisReport> Reports { get; }

        //TODO Make Readonly

        /// <summary>
        /// The registered analyzers.
        /// </summary>
        ObservableCollection<ISharperCryptoApiAnalysisAnalyzer> Analyzers { get; }

        /// <summary>
        /// Reports the specified report.
        /// </summary>
        /// <param name="report">The report.</param>
        void Report(IAnalysisReport report);

        /// <summary>
        /// Registers an analyzer.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        void RegisterAnalyzer(ISharperCryptoApiAnalysisAnalyzer analyzer);

        /// <summary>
        /// Tries the get report.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="report">The report.</param>
        /// <returns><see langword="true"/> if a report was found; <see langword="false"/> otherwise</returns>
        bool TryGetReport(string errorCode, out IAnalysisReport report);

        /// <summary>
        /// Pushes an report to the history.
        /// </summary>
        /// <param name="report">The report.</param>
        void PushHistory(IAnalysisReport report);
    }
}