using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class ConstantIvReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Compile-time constant IV";

        private static readonly LocalizableString DescriptionString =
            "'Not using a random initialization Vector (IV) Mode causes algorithms to be susceptible to dictionary attacks.'(CWE-329)";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.WeakConfiguration + "/" +
                                                    CommonAnalysisCategories.DiscoverableInformation;
        private static readonly LocalizableString Remarks =
            "Consider using the GenerateIV() method of the cryptographic provider.";

        private static readonly Exploitability ExploitabilityValue = Exploitability.Medium;

        private static SecurityGoals SecurityGoals = SecurityGoals.Confidentiality;

        private static readonly NamedLink HowToAttackLink = new NamedLink("How a constant IV could be attacked", new Uri("https://defuse.ca/cbcmodeiv.htm"));
        private static readonly NamedLink CweLink = new NamedLink("CWE-329", new Uri("https://cwe.mitre.org/data/definitions/329.html"));

        public ConstantIvReport() : base(AnalysisIds.ConstantIv, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), CweLink, HowToAttackLink)
        {
        }
    }
}
