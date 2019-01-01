using System;
using System.Globalization;
using System.Windows.Controls;
using SharperCryptoApiAnalysis.Shell.Interop.Validation;

namespace SharperCryptoApiAnalysis.Vsix.Ui.ValidationRules
{
    public class IsUriRule : ValidationRuleBase<string>
    {
        protected override ValidationResult Validate(string value, CultureInfo cultureInfo)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out var resultURI))
            {
                if (resultURI.Scheme == Uri.UriSchemeHttp || resultURI.Scheme == Uri.UriSchemeHttps)
                    return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Not a Uri. Please use http(s):// as uri scheme.");
        }
    }
}
