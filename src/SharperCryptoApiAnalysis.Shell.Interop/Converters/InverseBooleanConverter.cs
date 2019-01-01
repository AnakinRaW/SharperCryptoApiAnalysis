using System.Globalization;

namespace SharperCryptoApiAnalysis.Shell.Interop.Converters
{
    /// <summary>
    /// Inverts a boolean
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Shell.Interop.Converters.ValueConverter{System.Boolean, System.Boolean}" />
    public class InverseBooleanConverter : ValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }

        protected override bool ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}
