using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SharperCryptoApiAnalysis.Vsix.ViewModels.Extension;

namespace SharperCryptoApiAnalysis.Vsix.Controls
{
    internal class ExtensionListBox : ListBox
    {
        public static readonly DependencyProperty CheckBoxesEnabledProperty = DependencyProperty.Register(
            "CheckBoxesEnabled", typeof(bool), typeof(ExtensionListBox), new PropertyMetadata(default(bool)));

        public bool CheckBoxesEnabled
        {
            get => (bool) GetValue(CheckBoxesEnabledProperty);
            set => SetValue(CheckBoxesEnabledProperty, value);
        }

        static ExtensionListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtensionListBox),
                new FrameworkPropertyMetadata(typeof(ExtensionListBox)));
        }

        public ExtensionListBox()
        {
            Loaded += OnLoaded;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is LoadingStatusIndicator)
            {
                if (e.RemovedItems.Count > 0)
                    SelectedItem = e.RemovedItems[0];
                else
                    SelectedIndex = -1;
            }
            else
                base.OnSelectionChanged(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            var border = VisualTreeHelper.GetChild(this, 0) as Border;
            if (border == null)
                return;
            border.Padding = new Thickness(0);
            var scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
            if (scrollViewer == null)
                return;
            scrollViewer.Padding = new Thickness(0);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            if (SelectedItem is ExtensionItemListViewModel item && e.Key == Key.Space)
            {
                item.Selected = !item.Selected;
                e.Handled = true;
            }
        }
    }
}
