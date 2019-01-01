using System;
using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels
{
    [Export(typeof(IAnalyzerReportsViewModel))]
    internal class AnalyzerReportsViewModel : PanePageViewModelBase, IAnalyzerReportsViewModel
    {
        private ISharperCryptoAnalysisServiceProvider _serviceProvider;

        public IAnalyzerManager AnalyzerManager { get; }

        public override string Title => "Reports";

        [ImportingConstructor]
        public AnalyzerReportsViewModel(ISharperCryptoAnalysisServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            AnalyzerManager = _serviceProvider.GetService<IAnalyzerManager>();
        }
    }
}
