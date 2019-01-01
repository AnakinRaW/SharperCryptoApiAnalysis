using System.Globalization;
using System.Windows;
using SharperCryptoApiAnalysis.Shell.Interop.Converters;

namespace SharperCryptoApiAnalysis.Vsix.Ui.Converters
{
    internal class StringNotEmptyToVisibilityConverter : ValueConverter<string, Visibility>
    {
        protected override Visibility Convert(string value, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
