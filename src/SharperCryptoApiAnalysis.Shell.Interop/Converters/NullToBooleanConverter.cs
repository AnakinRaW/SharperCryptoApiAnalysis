using System.Globalization;

namespace SharperCryptoApiAnalysis.Shell.Interop.Converters
{
    /// <summary>
    /// If the value is null this converter will return <see langword="true"/>
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Shell.Interop.Converters.ValueConverter{System.Object, System.Boolean}" />
    public class NullToBooleanConverter : ValueConverter<object, bool>
    {
        protected override bool Convert(object value, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        protected override object ConvertBack(bool value, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
