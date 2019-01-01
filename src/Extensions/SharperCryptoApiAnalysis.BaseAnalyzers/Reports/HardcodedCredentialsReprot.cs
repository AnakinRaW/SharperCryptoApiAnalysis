using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class HardcodedCredentialsReprot : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Hardcoded credentials";

        private static readonly LocalizableString DescriptionString =
            "During development you might want quick access to known credentials for testing purposes. In many cases programmers forget to remove this code " +
            "or just disabled it. However an attacker is able to decompile .NET application entirely, allowing them to gain any information you have written in the code, " +
            "disabled or not. ";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.DiscoverableInformation;
        private static readonly LocalizableString Remarks =
            "Instead of storing the password or even it's derived hash as a string in the code please consider the following option:\r\n" +
            "Store the password in a file or on a server that's only available to you and read it from there";

        private static readonly Exploitability ExploitabilityValue = Exploitability.High;

        private static SecurityGoals SecurityGoals = SecurityGoals.Confidentiality | SecurityGoals.Availability | SecurityGoals.Integrity;

        private static readonly NamedLink HowToAttackLink = new NamedLink("C# decompiler", new Uri("https://github.com/icsharpcode/ILSpy#ilspy-------"));
        private static readonly NamedLink CweLink = new NamedLink("CWE-Entry about Hardcoded passwords", new Uri("http://cwe.mitre.org/data/definitions/259"));
        private static readonly NamedLink Cwe2Link = new NamedLink("CWE-Entry about Hardcoded credentials", new Uri("http://cwe.mitre.org/data/definitions/798"));

        public HardcodedCredentialsReprot() : base(AnalysisIds.HardcodedCredentials, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), CweLink, Cwe2Link, HowToAttackLink)
        {
        }
    }
}
