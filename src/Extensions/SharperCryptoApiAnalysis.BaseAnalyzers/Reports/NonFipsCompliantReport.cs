using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class NonFipsCompliantReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Specified provider is not FIPS 140-2 compliant";

        private static readonly LocalizableString DescriptionString = "Governments and some companies require the usage of FIPS 140-2 certified algorithms to perform cryptographic tasks. " +
                                                                      "The .NET offers such implementations along side with non compliant versions. Those non compliant implementations usually " +
                                                                      "have the suffix 'Managed' in the class name (e.g.: SHA512Managed). The FIPS compliant versions are implemented by the operating system and accessed via internal wrappers.";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.WeakConfiguration + "/" +
                                                    CommonAnalysisCategories.DiscoverableInformation;
        private static readonly LocalizableString Remarks = "In general you can use any implementation that ends with CryptoServiceProvider or Cng (e.g.: AesCryptoServiceProvider or " +
                                                            "SHA512CryptoServiceProvider)";

        private static readonly Exploitability ExploitabilityValue = Exploitability.Theoretical;

        private static SecurityGoals SecurityGoals = SecurityGoals.None;

        private static readonly NamedLink Microsoft = new NamedLink("Microsoft FIPS 140-2 validation", new Uri("https://docs.microsoft.com/en-us/windows/security/threat-protection/fips-140-validation#ID0EWFAC"));

        public NonFipsCompliantReport() : base(AnalysisIds.NonFipsCompliant, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), Microsoft)
        {
        }
    }
}
