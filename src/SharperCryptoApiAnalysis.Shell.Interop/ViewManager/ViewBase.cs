using System.Windows;
using System.Windows.Controls;

namespace SharperCryptoApiAnalysis.Shell.Interop.ViewManager
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for views.
    /// </summary>
    public class ViewBase<TInterface, TImplementor> : UserControl
        where TInterface : class, IViewModel
        where TImplementor : class
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(TInterface), typeof(TImplementor), new PropertyMetadata(null, ViewModelChanged));

        private bool _dataContextChanged;

        /// <summary>
        /// Gets or sets the control's data context as a typed view model. Required for interaction
        /// with ReactiveUI.
        /// </summary>
        public TInterface ViewModel
        {
            get => (TInterface) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public ViewBase()
        {
            DataContextChanged += OnDataContextChanged;
        }

        private static void ViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ViewBase<TInterface, TImplementor> viewBase))
                return;
            viewBase.ViewModelChanged(e);
        }

        private void OnDataContextChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            _dataContextChanged = true;
            ViewModel = (TInterface) e.NewValue;
            _dataContextChanged = false;
        }

        private void ViewModelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_dataContextChanged)
                return;
            DataContext = e.NewValue;
        }
    }
}