using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class ShortSaltSizeReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Salt size is too short";

        private static readonly LocalizableString DescriptionString =
            "Using a too short salt size does not add too much value. The salt is appended to the password to make it unique and therefore not able to lookup in a " +
            "dictionary or rainbow table. You could say that for each byte (e.g. an ASCII char) added to the salt the attacker needs 256^n dictionaries. " +
            "The .NET required you to use at least 64 bits (8 bytes) of salt size. Values below will trow an exception";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.WeakConfiguration;

        private static readonly LocalizableString Remarks =
            "The minimum salt size of this analyzer is 16 bytes as suggested by the NIST.";

        private static readonly Exploitability ExploitabilityValue = Exploitability.Medium;

        private static SecurityGoals SecurityGoals = SecurityGoals.Confidentiality;

        private static readonly NamedLink Nist = new NamedLink("Read the NIST guidance", new Uri("https://nvlpubs.nist.gov/nistpubs/Legacy/SP/nistspecialpublication800-132.pdf"));
        private static readonly NamedLink RainbowTable = new NamedLink("What is a rainbow table", new Uri("https://en.wikipedia.org/wiki/Rainbow_table"));
        private static readonly NamedLink CweLink = new NamedLink("More about on password derivation and salt", new Uri("https://crackstation.net/hashing-security.htm#salt"));

        public ShortSaltSizeReport() : base(AnalysisIds.ShortSaltSize, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), Nist, CweLink, RainbowTable)
        {
        }
    }
}
