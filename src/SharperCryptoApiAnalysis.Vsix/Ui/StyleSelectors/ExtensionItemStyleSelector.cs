using System.Windows;
using System.Windows.Controls;
using SharperCryptoApiAnalysis.Vsix.ViewModels.Extension;

namespace SharperCryptoApiAnalysis.Vsix.Ui.StyleSelectors
{
    internal class ExtensionItemStyleSelector : StyleSelector
    {
        public Style ItemStyle { get; set; }

        public Style StatusIndicatorStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is LoadingStatusIndicator)
                return StatusIndicatorStyle;
            return ItemStyle;
        }
    }
}
