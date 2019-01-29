using System;
using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class WeakHashingReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Weak or broken hashing function used.";

        private static readonly LocalizableString DescriptionString = "In many situation hashes are used to store passwords or ensure information integrity. " +
                                                                      "Therefore a hash function must be collision free. Otherwise attackers can guess, generate or brute force data that produces the same hash. " +
                                                                      "An attacker can perform different kinds of attacks like preimage attack, second preimage attack or collision attacks. " +
                                                                      "It has been proved that the MD5 hash function is vulnerable to collision attacks. " +
                                                                      "Though RIPEMD-160, which .NET provides, seems to be safe but is not in common use and therefore not well investigated for flaws. Thus it should not be used. " +
                                                                      "\r\nNotice that the provided CVE reports links, regarding MD5, indicate each vulnerability with a critical score.";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.WeakConfiguration;

        private static readonly LocalizableString Remarks =
            "To ensure you are using a secure hashing function please use any subclass from SHA256, SHA384 or SHA 512 such as SHA256CryptoServiceProvider. " +
            "NOTE: The implementations of encryption algorithms implement the IDisposable interface and therefore need to get closed properly, e.g. in a using block.";

        private static readonly Exploitability ExploitabilityValue = Exploitability.Medium;

        private static SecurityGoals SecurityGoals = SecurityGoals.Integrity | SecurityGoals.Confidentiality;

        private static readonly NamedLink Nist= new NamedLink("NIST recommendation for Hashing (Chapter 9)", new Uri("https://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-131Ar1.pdf"));
        private static readonly NamedLink CveLink = new NamedLink("CVE Report where MD5 was used to hash passwords", new Uri("https://nvd.nist.gov/vuln/detail/CVE-2018-16705"));
        private static readonly NamedLink CveLink2 = new NamedLink("Another CVE Report where MD5 was used to hash passwords", new Uri("https://nvd.nist.gov/vuln/detail/CVE-2016-9488"));
        private static readonly NamedLink Md5Collision = new NamedLink("How MD5 collisions work", new Uri("https://www.mscs.dal.ca/~selinger/md5collision/"));


        public WeakHashingReport() : base(AnalysisIds.WeakHashing, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString(), Nist, CveLink, CveLink2, Md5Collision)
        {
        }
    }
}
