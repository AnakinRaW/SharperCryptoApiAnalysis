using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;

namespace SharperCryptoApiAnalysis.Shell.ViewPattern
{
    [Export(typeof(IViewModelFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ViewModelFactory : IViewModelFactory
    {
        readonly ISharperCryptoAnalysisServiceProvider _serviceProvider;

        [ImportingConstructor]
        public ViewModelFactory(ISharperCryptoAnalysisServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [ImportMany]
        IEnumerable<ExportFactory<FrameworkElement, IViewModelMetadata>> Views { get; set; }

        public TViewModel CreateViewModel<TViewModel>() where TViewModel : IViewModel
        {
            return _serviceProvider.ExportProvider.GetExport<TViewModel>().Value;
        }

        public FrameworkElement CreateView<TViewModel>() where TViewModel : IViewModel
        {
            return CreateView(typeof(TViewModel));
        }

        public FrameworkElement CreateView(Type viewModel)
        {
            var f = Views.FirstOrDefault(x => x.Metadata.ViewModelType.Contains(viewModel.FullName));
            return f?.CreateExport().Value;
        }
    }
}
