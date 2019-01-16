using System.Globalization;
using Microsoft.VisualStudio.PlatformUI;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Vsix.Ui.Converters
{
    internal class InverseBaseExtensionToBooleanConverter : ValueConverter<ISharperCryptoApiExtensionMetadata, bool>
    {
        protected override bool Convert(ISharperCryptoApiExtensionMetadata value, object parameter, CultureInfo culture)
        {
            return !value.Name.Equals(Constants.DefaultAnalyzerAssemblyName);
        }
    }
}
