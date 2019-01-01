using System.Windows;
using System.Windows.Controls;

namespace SharperCryptoApiAnalysis.Vsix.Controls
{
    public class ReportsListBox : ListBox
    {
        static ReportsListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportsListBox),
                new FrameworkPropertyMetadata(typeof(ReportsListBox)));
        }
    }
}
