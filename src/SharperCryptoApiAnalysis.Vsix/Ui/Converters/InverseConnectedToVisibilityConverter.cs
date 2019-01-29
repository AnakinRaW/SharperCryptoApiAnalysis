using System.Globalization;
using System.Windows;
using SharperCryptoApiAnalysis.Shell.Interop.Converters;

namespace SharperCryptoApiAnalysis.Vsix.Ui.Converters
{
    internal class InverseConnectedToVisibilityConverter : ValueConverter<bool, Visibility>
    {
        protected override Visibility Convert(bool value, object parameter, CultureInfo culture)
        {
            return value ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
