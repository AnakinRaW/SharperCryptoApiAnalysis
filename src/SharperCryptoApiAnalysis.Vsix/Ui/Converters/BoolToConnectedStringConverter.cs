using System.Globalization;
using SharperCryptoApiAnalysis.Shell.Interop.Converters;

namespace SharperCryptoApiAnalysis.Vsix.Ui.Converters
{
    internal class BoolToConnectedStringConverter : ValueConverter<bool, string>
    {
        protected override string Convert(bool value, object parameter, CultureInfo culture)
        {
            return value ? "Connected" : "Disconnected";
        }
    }
}
