using System.Windows;
using System.Windows.Controls;

namespace SharperCryptoApiAnalysis.Vsix.Controls
{
    internal class ExtensionItemControl : ContentControl
    {
        static ExtensionItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtensionItemControl),
                new FrameworkPropertyMetadata(typeof(ExtensionItemControl)));
        }
    }
}
