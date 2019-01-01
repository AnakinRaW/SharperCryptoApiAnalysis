using System;
using System.Threading.Tasks;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;

namespace SharperCryptoApiAnalysis.Shell.ViewModels
{
    public interface ISharperCryptoApiAnalysisPaneViewModel : IViewModel
    {
        bool IsSearchEnabled { get; }

        string SearchQuery { get; set; }

        IViewModel Content { get; }

        Task InitializeAsync(IServiceProvider paneServiceProvider);

        void ShowView<T>() where T : class, IViewModel;
    }
}