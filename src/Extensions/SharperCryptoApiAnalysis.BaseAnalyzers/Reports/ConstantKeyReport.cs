using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class ConstantKeyReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Hardcoded encryption key";

        private static readonly LocalizableString DescriptionString =
            "The key of any symmetric encryption mus be kept secret if you want to protect your data. If the key is known by an attacker it is almost granted he is able to decipher them. " +
            "A hardcoded encryption key is easy to steal by decompiling .NET assemblies.";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.DiscoverableInformation;
        private static readonly LocalizableString Remarks =
            "Depending on your requirements consider using the GenerateKey() Method of the cipher if you do not rely on password based encryption. " +
            "If you need password based encryption consider using the code generation this tool provides.";

        private static readonly Exploitability ExploitabilityValue = Exploitability.High;

        private static SecurityGoals SecurityGoals = SecurityGoals.Confidentiality | SecurityGoals.Integrity;

        private static readonly NamedLink CweLink = new NamedLink("CWE-Entry about Hardcoded passwords", new Uri("http://cwe.mitre.org/data/definitions/259"));

        public ConstantKeyReport() : base(AnalysisIds.ConstantKey, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), CweLink)
        {
        }
    }
}
