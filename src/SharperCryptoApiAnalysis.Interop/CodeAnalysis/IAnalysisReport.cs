using System;
using System.Collections.Generic;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis
{
    public interface IAnalysisReport
    {
        /// <summary>
        /// Contains custom data to be displayed in a reports viewer
        /// <remarks>Object should be a UI component like FrameworkElement</remarks>
        /// </summary>
        object AdditionalContent { get; }
        /// <summary>
        /// The category this analysis belongs to.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// The description of the report
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The identifier of the report
        /// </summary>
        string Id { get; }

        /// <summary>
        /// URL leading to a even more detailed report.
        /// </summary>
        Uri MoreDetailsUrl { get; }

        /// <summary>
        /// Related links to the report.
        /// </summary>
        IEnumerable<NamedLink> RelatedLinks { get; }

        /// <summary>
        /// The exploitability of the weakness or threat.
        /// </summary>
        Exploitability Exploitability { get; }

        /// <summary>
        /// Information how to fix the code.
        /// </summary>
        string SolutionRemarks { get; }

        /// <summary>
        /// Short summary of the report.
        /// </summary>
        string Summary { get; }

        /// <summary>
        /// Security goals like Authenticity or Integrity which could get exposed
        /// </summary>
        SecurityGoals ExposedSecurityGoals { get; }
    }
}