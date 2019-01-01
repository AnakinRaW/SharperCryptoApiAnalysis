using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class ConstantSaltReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Compile-time constant Salt";

        private static readonly LocalizableString DescriptionString =
            "Not using a unique salt foreach encryption does not add any value because all derived keys depend on the same salt. It make the salt principle obsolete." +
            "Imagine multiple users use the same password but you derive the secret key from the same salt. the result will be that every key is the same." +
            "This allows an attacker to perform a dictionary or rainbow table attacks on all the keys you derived with that salt.";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.WeakConfiguration;

        private static readonly LocalizableString Remarks =
            "The Rfc2898DeriveBytes class owns a constructor that does not require you to specify a salt but just the size of the salt. The salt will be generated automatically. Consider using this constructor instead.";

        private static readonly Exploitability ExploitabilityValue = Exploitability.Low;

        private static SecurityGoals SecurityGoals = SecurityGoals.Confidentiality;

        private static readonly NamedLink RainbowTable = new NamedLink("What is a rainbow table", new Uri("https://en.wikipedia.org/wiki/Rainbow_table"));
        private static readonly NamedLink CweLink = new NamedLink("More about on password derivation and salt", new Uri("https://crackstation.net/hashing-security.htm#salt"));

        public ConstantSaltReport() : base(AnalysisIds.ConstantSalt, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), CweLink, RainbowTable)
        {
        }
    }
}
