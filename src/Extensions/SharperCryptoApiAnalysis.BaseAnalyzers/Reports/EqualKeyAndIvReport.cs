using Microsoft.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Reports
{
    public class EqualKeyAndIvReport : AnalysisReport
    {
        private static readonly LocalizableString SummaryString = "Equal IV and Key in symmetric encryption";

        private static readonly LocalizableString DescriptionString =
            "The IV was created to add more randomness to encryption so the same plain texts with the same key produce different secret texts (cipher text). " +
            "Making IV and key equal on the one hand make the IV obsolete but on the other hand it exposes the key as well, because the IV is generally knows by anybody " +
            "so the decryption can be performed.";

        private static readonly LocalizableString CategoryString = CommonAnalysisCategories.WeakConfiguration;
        private static readonly LocalizableString Remarks =
            "Assign different values to IV and Key Properties by using the dedicated GenerateIV() and GenerateKey() methods. " +
            "If you plan to implement password based encryption you need to specify the key manually. Not that you should neither hardcode the password or key nor " +
            "use the password directly as the key.";

        private static readonly Exploitability ExploitabilityValue = Exploitability.High;

        private static SecurityGoals SecurityGoals = SecurityGoals.Confidentiality | SecurityGoals.Integrity;

        public EqualKeyAndIvReport() : base(AnalysisIds.EqualKeyAndIv, SummaryString.ToString(), DescriptionString.ToString(), CategoryString.ToString(),
            ExploitabilityValue, SecurityGoals, null, Remarks.ToString())
        {
        }
    }
}
