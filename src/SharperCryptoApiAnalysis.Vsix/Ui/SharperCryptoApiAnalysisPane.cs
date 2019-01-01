using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.ViewModels;
using SharperCryptoApiAnalysis.Shell.ViewPattern;

namespace SharperCryptoApiAnalysis.Vsix.Ui
{
    [Guid(SharperCryptoAnalysisPaneGuid)]
    public class SharperCryptoApiAnalysisPane : ToolWindowPane
    {
        public const string SharperCryptoAnalysisPaneGuid = "c6e07d2c-832a-4466-bf16-bd8aeeb394f7";

        private JoinableTask<ISharperCryptoApiAnalysisPaneViewModel> _viewModelTask;
        private readonly ContentPresenter _contentPresenter;
        private IDisposable _viewSubscription;

        public FrameworkElement View
        {
            get => _contentPresenter.Content as FrameworkElement;
            set
            {
                _viewSubscription?.Dispose();
                _viewSubscription = null;

                if (_contentPresenter.Content is FrameworkElement fe)
                    fe.DataContextChanged -= ValueOnDataContextChanged;
                _contentPresenter.Content = value;
                value.DataContextChanged += ValueOnDataContextChanged;        
            }
        }

        private void ValueOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is ISharperCryptoApiAnalysisPaneViewModel propertyChanged)
                propertyChanged.PropertyChanged -= OnDataContextPropertyChanged;
            if (e.NewValue != null && e.NewValue is ISharperCryptoApiAnalysisPaneViewModel newValue)
            {
                newValue.PropertyChanged += OnDataContextPropertyChanged;
                UpdateSearchHost(false, null);
            }
        }

        private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISharperCryptoApiAnalysisPaneViewModel.IsSearchEnabled) ||
                e.PropertyName == nameof(ISharperCryptoApiAnalysisPaneViewModel.SearchQuery))
                UpdateSearchHost(false, null);
        }

        public override bool SearchEnabled => false;

        public SharperCryptoApiAnalysisPane() : base(null)
        {
            Caption = "Sharper Crypto-API Analysis";
            Content = _contentPresenter = new ContentPresenter();

            //BitmapImageMoniker = new ImageMoniker
            //{
            //    Guid = new Guid("{670405d6-ff01-49b5-a000-e89806716e6a}"),
            //    Id = 1
            //};

            ToolBar = new CommandID(PackageGuids.ToolbarCommandSet, PackageIds.SharperCryptoApiAnalysisToolbar);
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }

        public Task<ISharperCryptoApiAnalysisPaneViewModel> GetViewModelAsync() => _viewModelTask.JoinAsync();

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();
            Marshal.ThrowExceptionForHR(((IVsWindowFrame)Frame)?.SetProperty(
                                            (int)__VSFPROPID5.VSFPROPID_SearchPlacement,
                                            __VSSEARCHPLACEMENT.SP_NONE) ?? 0);
            var pane = View?.DataContext as ISharperCryptoApiAnalysisPaneViewModel;
            UpdateSearchHost(pane?.IsSearchEnabled ?? false, pane?.SearchQuery);
        }

        protected override void Initialize()
        {
            var asyncPackage = (AsyncPackage)Package;
            _viewModelTask = asyncPackage.JoinableTaskFactory.RunAsync(() => InitializeAsync(asyncPackage));
        }

        private async Task<ISharperCryptoApiAnalysisPaneViewModel> InitializeAsync(AsyncPackage asyncPackage)
        {
            try
            {
                ShowInitializing();

                var provider =
                    (ISharperCryptoAnalysisServiceProvider)await asyncPackage.GetServiceAsync(
                        typeof(ISharperCryptoAnalysisServiceProvider));

                var factory = provider.GetService<IViewModelFactory>();
                var viewModel = provider.ExportProvider.GetExportedValue<ISharperCryptoApiAnalysisPaneViewModel>();
                await viewModel.InitializeAsync(this);

                View = factory.CreateView<ISharperCryptoApiAnalysisPaneViewModel>();
                View.DataContext = viewModel;
                return viewModel;
            }
            catch (Exception e)
            {
                ShowError(e);
                throw;
            }

        }

        private void ShowInitializing()
        {
            // This page is intentionally left blank.
        }

        private void ShowError(Exception e)
        {
            View = new TextBox
            {
                Text = e.ToString(),
                IsReadOnly = true,
            };
        }

        private void UpdateSearchHost(bool enabled, string query)
        {
            if (SearchHost != null && enabled)
            {
                SearchHost.IsEnabled = true;
                if (SearchHost.SearchQuery?.SearchString != query)
                {
                    SearchHost.SearchAsync(/*query != null ? new SearchQuery(query) : */null);
                }
            }
        }
    }
}
