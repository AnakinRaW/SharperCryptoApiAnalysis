using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class WeakOperationModeReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Weak operation mode used.";

        private static readonly LocalizableString DescriptionString = "The operation mode (or cipher) decides how the cipher will encrypt data. " +
                                                                      "While all of them have their different advantages like allowing to parallelize the encryption or decryption, " +
                                                                      "They also have known weakness the programmer must be aware of. " +
                                                                      "For instance the cipher mode OFB requires to use a different initialization vector for each message. " +
                                                                      "Probably most prominent for it's weaknesses is cipher mode ECB which should not be used.";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.WeakConfiguration;

        private static readonly LocalizableString Remarks = "The .NET Framework does (currently) not provide GCM or GCC as operation modes for encryption. " +
                                                            "Using ECB must be avoided! The best option so far is using CBC in combination with HMAC, " +
                                                            "as the included code generation task can produce. ";

        private static readonly Exploitability ExploitabilityValue = Exploitability.Medium;

        private static SecurityGoals SecurityGoals = SecurityGoals.Confidentiality;

        private static readonly NamedLink Ecb= new NamedLink("How an attack on ECB works", new Uri("https://zachgrace.com/posts/attacking-ecb/"));
        private static readonly NamedLink Hmac= new NamedLink("HMAC on Wikipedia", new Uri("https://en.wikipedia.org/wiki/HMAC"));
        private static readonly NamedLink Padding = new NamedLink("Padding oracle attack on CBC explained", new Uri("https://blog.cloudflare.com/yet-another-padding-oracle-in-openssl-cbc-ciphersuites/"));
        private static readonly NamedLink Cbc = new NamedLink("Microsoft guide for using CBC mode ", new Uri("https://docs.microsoft.com/de-de/dotnet/standard/security/vulnerabilities-cbc-mode"));


        public WeakOperationModeReport() : base(AnalysisIds.WeakOperationMode, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), Cbc, Ecb, Padding, Hmac)
        {
        }
    }
}
