using System.Globalization;
using System.Windows;

namespace SharperCryptoApiAnalysis.Shell.Interop.Converters
{
    /// <summary>
    /// If the value is null the converter will return a collapsed visibility
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Shell.Interop.Converters.ValueConverter{System.Object, System.Windows.Visibility}" />
    public class NullToVisibilityConverter : ValueConverter<object, Visibility>
    {
        protected override Visibility Convert(object value, object parameter, CultureInfo culture)
        {
            return (Visibility)(value == null ? 2 : 0);
        }
    }
}
