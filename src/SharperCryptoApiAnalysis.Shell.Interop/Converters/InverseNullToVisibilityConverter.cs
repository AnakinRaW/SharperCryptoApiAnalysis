using System.Globalization;
using System.Windows;

namespace SharperCryptoApiAnalysis.Shell.Interop.Converters
{
    /// <summary>
    /// Visible when value is null
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Shell.Interop.Converters.ValueConverter{System.Object, System.Windows.Visibility}" />
    public class InverseNullToVisibilityConverter : ValueConverter<object, Visibility>
    {
        protected override Visibility Convert(object value, object parameter, CultureInfo culture)
        {
            return (Visibility) (value == null ? 0 : 2);
        }
    }
}
