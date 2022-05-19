using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.PlatformUI;
using SharperCryptoApiAnalysis.Shell.Commands;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;
using SharperCryptoApiAnalysis.Shell.ViewPattern;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels
{
    [Export(typeof(ISharperCryptoApiAnalysisPaneViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SharperCryptoApiApiAnalysisPaneViewModel : ViewModelBase, ISharperCryptoApiAnalysisPaneViewModel
    {
        public bool IsSearchEnabled { get; }

        private string _searchQuery;
        private IViewModel _content;
        private Task _initializeTask;
        private string _title;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (value == _searchQuery)
                    return;
                _searchQuery = value;
                OnPropertyChanged();
            }
        }
        public IViewModel Content
        {
            get => _content;
            private set
            {
                if (value == _content)
                    return;

                if (_content is IPanePageViewModel oldViewModel)
                    oldViewModel.Deactivated();

                _content = value;
                OnPropertyChanged();

                if (_content is IPanePageViewModel pageViewModel)
                    pageViewModel.Activated();
            }
        }

        private ICommand ShowManageConnectionsCommand => new DelegateCommand(ShowView<IManageConnectionsViewModel>);
        private ICommand ShowManageExtensionsCommand => new DelegateCommand(ShowView<IManageExtensionsViewModel>);
        private ICommand ShowAnalyzerDetailCommand => new DelegateCommand(ShowView<IAnalyzerDetailViewModel>);
        private ICommand ShowAnalyzerReportsCommand => new DelegateCommand(ShowView<IAnalyzerReportsViewModel>);

        [ImportingConstructor]
        public SharperCryptoApiApiAnalysisPaneViewModel(
            IViewModelFactory viewModelFactory,
            IManageConnectionsViewModel connectionsViewModel)
        {
            Validate.IsNotNull(viewModelFactory, nameof(viewModelFactory));

            if (connectionsViewModel != null)
                _content = connectionsViewModel;

            IsSearchEnabled = false;
        }

        public Task InitializeAsync(IServiceProvider paneServiceProvider)
        {
            return _initializeTask = _initializeTask ?? CreateInitializeTask(paneServiceProvider);
        }

        private Task CreateInitializeTask(IServiceProvider paneServiceProvider)
        {
            var menuService = (IMenuCommandService) paneServiceProvider.GetService(typeof(IMenuCommandService));
            BindNavigationCommand(menuService, PackageIds.ToolbarAnalyzerReportsId, ShowAnalyzerReportsCommand);
            BindNavigationCommand(menuService, PackageIds.ToolbarAnalyzerDetailId, ShowAnalyzerDetailCommand);
            BindNavigationCommand(menuService, PackageIds.ToolbarManageConnectionsId, ShowManageConnectionsCommand);
            BindNavigationCommand(menuService, PackageIds.ToolbarManageExtensionsId, ShowManageExtensionsCommand);   
            return Task.CompletedTask;
        }

        private void BindNavigationCommand(IMenuCommandService menu, int commandId, ICommand command)
        {
            Validate.IsNotNull(menu, nameof(menu));
            Validate.IsNotNull(command, nameof(command));
            menu.BindCommand(new CommandID(PackageGuids.ToolbarCommandSet, commandId), command);
        }

        public void ShowView<T>() where T : class, IViewModel
        {
            var viewModel = VisualStudio.Integration.Services.SharperCryptoAnalysisServiceProvider.GetService<T>();
            Content = viewModel;
        }
    }
}
