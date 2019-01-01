using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;
using SharperCryptoApiAnalysis.VisualStudio.Integration;

namespace SharperCryptoApiAnalysis.Shell.ViewPattern
{
    public class ViewLocator : IValueConverter
    {
        private static IViewModelFactory _factoryProvider;

        /// <summary>
        /// Converts a view model into a view.
        /// </summary>
        /// <param name="value">The view model.</param>
        /// <param name="targetType">Unused.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>
        /// A new instance of a view for the specified view model, or an error string if a view
        /// could not be located.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exportViewModelAttribute = value.GetType()
                .GetCustomAttributes(typeof(ExportAttribute), false)
                .OfType<ExportAttribute>()
                .FirstOrDefault(x => typeof(IViewModel).IsAssignableFrom(x.ContractType));

            if (exportViewModelAttribute != null)
            {
                var view = Factory?.CreateView(exportViewModelAttribute.ContractType);

                if (view != null)
                {
                    var result = view;
                    result.DataContext = value;
                    return result;
                }
            }

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
#endif

            return FormattableString.Invariant($"Could not locate view for '{value.GetType()}'");
        }

        /// <inheritdoc />
        /// <summary>
        /// Not implemented in this class.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static IViewModelFactory Factory => _factoryProvider ?? (_factoryProvider =
                                                        Services.SharperCryptoAnalysisServiceProvider.TryGetService<IViewModelFactory>());
    }
}
