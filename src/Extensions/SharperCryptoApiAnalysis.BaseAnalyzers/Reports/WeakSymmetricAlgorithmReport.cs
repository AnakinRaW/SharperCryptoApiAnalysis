using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class WeakSymmetricAlgorithmReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Weak or broken symmetric encryption used.";

        private static readonly LocalizableString DescriptionString =
            "Some encryption algorithms, also called ciphers, have been proven to be vulnerable to attacks like Brute Force and Known Plaintext Attacks. " +
            "Some reasons are due to too short block or key sizes. " +
            "The .NET API provides you three of these vulnerable ciphers which are DES, Triple DES (3DES) and RC2.";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.WeakConfiguration;

        private static readonly LocalizableString Remarks =
            "To ensure you are using a secure cipher please use any subclass from AES or Rijndael such as AesCryptoServiceProvider. " +
            "NOTE: The implementations of encryption algorithms implement the IDisposable interface and therefore need to get closed properly, e.g. in a using block.";

        private static readonly Exploitability ExploitabilityValue = Exploitability.Medium;

        private static SecurityGoals SecurityGoals = SecurityGoals.Confidentiality;

        private static readonly NamedLink CweLink = new NamedLink("Read the CWE Entry", new Uri("https://cwe.mitre.org/data/definitions/327.html"));
        private static readonly NamedLink TDesExpliot = new NamedLink("How 3DS can be exploited", new Uri("https://www.synopsys.com/blogs/software-security/sweet32-retire-3des/"));


        public WeakSymmetricAlgorithmReport() : base(AnalysisIds.WeakSymmetricAlgorithm, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), CweLink, TDesExpliot)
        {
        }
    }
}
