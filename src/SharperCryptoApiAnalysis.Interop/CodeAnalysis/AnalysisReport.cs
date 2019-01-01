using System;
using System.Collections.Generic;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis
{
    public class AnalysisReport : IEquatable<IAnalysisReport>, IAnalysisReport
    {
        public static IAnalysisReport EmptyReport => new AnalysisReport();

        /// <summary>
        /// The identifier of the report
        /// </summary>
        public string Id { get; } = string.Empty;

        /// <summary>
        /// Short summary of the report.
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// URL leading to a even more detailed report.
        /// </summary>
        public Uri MoreDetailsUrl { get; }

        /// <summary>
        /// The description of the report
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The category this analysis belongs to.
        /// </summary>
        public string Category { get; }

        //public string Severity { get; }

        /// <summary>
        /// The exploitability of the weakness or threat.
        /// </summary>
        public Exploitability Exploitability { get; }

        /// <summary>
        /// Security goals like Authenticity or Integrity which could get exposed
        /// </summary>
        public SecurityGoals ExposedSecurityGoals { get; }

        /// <summary>
        /// Information how to fix the code.
        /// </summary>
        public string SolutionRemarks { get; }

        /// <summary>
        /// Related links to the report.
        /// </summary>
        public IEnumerable<NamedLink> RelatedLinks { get; }

        /// <summary>
        /// Contains custom data to be displayed in a reports viewer
        /// <remarks>Object should be a UI component like FrameworkElement</remarks>
        /// </summary>
        public object AdditionalContent { get; }

        private AnalysisReport()
        {  
        }

        protected AnalysisReport(string id)
        {
            Id = id;
        }

        public AnalysisReport(string id, string summary, string description, string category, /*string severity*/ Exploitability exploitability, SecurityGoals threats, Uri moreDetails, string solutionRemarks = null,
            params NamedLink[] relatedLinks) : this(id, summary, description, category, /*severity*/ exploitability, threats, moreDetails, solutionRemarks, null, relatedLinks)
        {
        }

        public AnalysisReport(string id, string summary, string description, string category, /*string severity,*/ Exploitability exploitability,  SecurityGoals threats, Uri moreDetails, 
            string solutionRemarks = null, object additionalContent = null,
           params NamedLink[] relatedLinks)
        {
            if (id == null || string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            Id = id;
            Summary = summary;
            Description = description;
            Category = category;
            Exploitability = exploitability;
            //Severity = severity;
            ExposedSecurityGoals = threats;
            SolutionRemarks = solutionRemarks;
            RelatedLinks = relatedLinks;
            AdditionalContent = additionalContent;
            MoreDetailsUrl = moreDetails;
        }

        public bool Equals(IAnalysisReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IAnalysisReport) obj);
        }

        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }
    }
}