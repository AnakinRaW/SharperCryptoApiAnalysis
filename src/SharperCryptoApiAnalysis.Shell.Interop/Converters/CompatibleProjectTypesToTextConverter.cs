using System.Globalization;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;

namespace SharperCryptoApiAnalysis.Shell.Interop.Converters
{
    /// <summary>
    /// Converts an <see cref="CompatibleProjectTypes"/> to a text value
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Shell.Interop.Converters.ValueConverter{SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator.CompatibleProjectTypes, System.String}" />
    public class CompatibleProjectTypesToTextConverter : ValueConverter<CompatibleProjectTypes, string>
    {
        protected override string Convert(CompatibleProjectTypes value, object parameter, CultureInfo culture)
        {
            var result = string.Empty;
            if (value.HasFlag(CompatibleProjectTypes.Framework))
                result += ".NetFramework; ";
            if (value.HasFlag(CompatibleProjectTypes.Core))
                result += ".NetCore; ";
            if (value.HasFlag(CompatibleProjectTypes.StandardV1))
                result += ".NetStandard V1.0; ";
            if (value.HasFlag(CompatibleProjectTypes.StandardV2))
                result += ".NetStandard V2.0; ";
            return result.Remove(result.Length - 2);
        }
    }
}
