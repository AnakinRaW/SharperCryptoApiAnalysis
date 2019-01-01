using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class WeakRandomReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "System.Random is not cryptographically strong";

        private static readonly LocalizableString DescriptionString =
            "In .NET System.Random is a weak Pseudo-Random Number Generator (PRNG) for cryptographic usage. However the reason to prefer it is the requirement of faster computation " +
            "and to generate number from a seed. The default constructor of System.Random uses (in .NET Framework) the system time with a 15ms time frame.";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.ApiAbuse;

        private static readonly LocalizableString Remarks =
            "If you need a secure random number that is not biased by a seed like system time, you should consider using RNGCryptoServiceProvider. " +
            "NOTE: This class implements IDisposable and therefore needs to get closed properly, e.g. in a using block.";

        private static readonly Exploitability ExploitabilityValue = Exploitability.Low;

        private static SecurityGoals SecurityGoals = SecurityGoals.None;

        private static readonly NamedLink CweLink = new NamedLink("Read the CWE Entry", new Uri("https://cwe.mitre.org/data/definitions/338.html"));
        private static readonly NamedLink SoAnswer = new NamedLink("Read about flaws in System.Random", new Uri("https://stackoverflow.com/a/6842191"));
        private static readonly NamedLink SecureRandom = new NamedLink("How to use the Secure Random Generator in C#", new Uri("https://docs.microsoft.com/de-de/dotnet/api/system.security.cryptography.rngcryptoserviceprovider"));


        public WeakRandomReport() : base(AnalysisIds.WeakRandom, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), CweLink, SoAnswer, SecureRandom)
        {
        }
    }
}
