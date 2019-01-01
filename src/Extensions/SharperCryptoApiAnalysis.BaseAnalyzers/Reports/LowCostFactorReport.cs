using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class LowCostFactorReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Low iteration count for password derive function";

        private static readonly LocalizableString DescriptionString =
            "Hashing functions like SHA are designed to be fast computable. Generating password-hash dictionaries profit from this performance. To reduce the effect " +
            "key derivation functions use a cost factor, also called iteration count, that increases the necessary computation time. Due to Moor's law this cost factor has to be adjusted" +
            "from time to time. When PBKDF2 was proposed in 2000 a default iteration count of 1000 was suggested. However this number is not up to date anymore.";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.WeakConfiguration;

        private static readonly LocalizableString Remarks =
            "These days an iteration count with no less than 10.000 is recommended. You can use the 3rd constructor parameter to specify the number of iterations.";

        private static readonly Exploitability ExploitabilityValue = Exploitability.Low;

        private static SecurityGoals SecurityGoals = SecurityGoals.Confidentiality;

        private static readonly NamedLink Brutefoce = new NamedLink("How long does it take to brute force passwords with low cost factor", new Uri("https://cryptosense.com/blog/parameter-choice-for-pbkdf2/"));
        private static readonly NamedLink CweLink = new NamedLink("More about on cost factors and iteration count", new Uri("https://cwe.mitre.org/data/definitions/916.html"));
        private static readonly NamedLink Nist = new NamedLink("See NIST specification", new Uri("https://pages.nist.gov/800-63-3/sp800-63b.html#sec5"));

        public LowCostFactorReport() : base(AnalysisIds.LowCostFactor, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), CweLink, Nist, Brutefoce)
        {
        }
    }
}
