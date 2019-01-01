using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;

namespace SharperCryptoApiAnalysis.Shell.ViewModels
{
    public interface IAnalyzerDetailViewModel : IPanePageViewModel
    {
        IAnalysisReport ActiveReport { get; set; }
    }
    public interface IAnalyzerReportsViewModel : IPanePageViewModel
    {

    }
}