using System.Globalization;

namespace SharperCryptoApiAnalysis.Shell.Interop.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// Inverts a boolean
    /// </summary>
    /// <seealso cref="T:SharperCryptoApiAnalysis.Shell.Interop.Converters.ValueConverter`2" />
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
