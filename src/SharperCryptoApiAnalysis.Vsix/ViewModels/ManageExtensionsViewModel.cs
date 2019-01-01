using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using SharperCryptoApiAnalysis.Vsix.Ui;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using ModernApplicationFramework.Input.Command;
using SharperCryptoApiAnalysis.Interop.Configuration;
using SharperCryptoApiAnalysis.Interop.Extensibility;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;
using SharperCryptoApiAnalysis.Vsix.ViewModels.Extension;
using InfoBarButton = Microsoft.VisualStudio.Shell.InfoBarButton;
using InfoBarModel = Microsoft.VisualStudio.Shell.InfoBarModel;
using InfoBarTextSpan = Microsoft.VisualStudio.Shell.InfoBarTextSpan;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels
{
    [Export(typeof(IManageExtensionsViewModel))]
    public class ManageExtensionsViewModel : PanePageViewModelBase, IManageExtensionsViewModel, IVsInfoBarUIEvents
    {
        private static readonly ImageMoniker InfoImageMoniker = KnownMonikers.StatusInformation;

        private readonly LoadingStatusIndicator _loadingStatusIndicator = new LoadingStatusIndicator();

        private static readonly JoinableTaskFactory JoinableTaskFactory = ThreadHelper.JoinableTaskFactory;

        private string _statusText = "Ready";
        private bool _checkBoxesEnabled;
        private ExtensionItemFilter _selectedFilter;
        private bool _selectAllItems;

        private IVsInfoBarUIElement _currentInfoBar;

        private uint _cookie;
        private IExtensionItemListViewModel _selectedItem;
        private IVsInfoBarHost _infoBarHost;

        public override string Title => "Manage Extensions";

        public IConfigurationManager ConfigurationManager { get; }
        
        public ObservableCollection<object> ItemsSource { get; }

        public IExtensionItemListViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(value, _selectedItem))
                    return;
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateCommand => new Command(ExecuteUpdate);
        public ICommand SyncCommand => new Command(ExecuteSync);

        public bool SelectAllItems
        {
            get => _selectAllItems;
            set
            {
                if (value == _selectAllItems)
                    return;
                _selectAllItems = value;
                OnPropertyChanged();
                ChangeSelectionOnAllItems(_selectAllItems);
            }
        }

        public ExtensionItemFilter SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (value == _selectedFilter) return;
                _selectedFilter = value;
                OnPropertyChanged();
                OnSelectedFilterChanged();
            }
        }

        public bool CheckBoxesEnabled
        {
            get => _checkBoxesEnabled;
            set
            {
                if (value == _checkBoxesEnabled) return;
                _checkBoxesEnabled = value;
                OnPropertyChanged();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                if (value == _statusText) return;
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public ManageExtensionsViewModel()
        {
            ConfigurationManager = VisualStudio.Integration.Services.SharperCryptoAnalysisServiceProvider
                .GetService<IConfigurationManager>();

            ItemsSource = new ObservableCollection<object>();
            ConfigurationManager.ExtensionManager.UpdateCompleted += OnUpdateCompleted;
            SelectedFilter = ExtensionItemFilter.Available;
        }

        public override void Activated()
        {
            base.Activated();
            OnPropertyChanged(nameof(SelectedFilter));
            OnSelectedFilterChanged();
        }

        private void OnUpdateCompleted(object sender, EventArgs e)
        {
            UpdateItems();

            var vs = VisualStudio.Integration.VisualStudio.GetInstance();
            vs.ClearComponentModelCache();
            vs.ClearExtensionCache();

            ShowRestartMessage();
        }

        private void ShowRestartMessage()
        {
            if (_currentInfoBar != null)
            {
                InfoBarHost.RemoveInfoBar(_currentInfoBar);
                _currentInfoBar = null;
            }

            var infoBar = new InfoBarModel(new[]
            {
                new InfoBarTextSpan(
                    "For the changes to take affect you need to restart Visual Studio. Do you want to restart now?")
            }, new[]
            {
                new InfoBarButton("Restart"),
            }, InfoImageMoniker);

            var sp = VisualStudio.Integration.Services.SharperCryptoAnalysisServiceProvider;
            IVsInfoBarUIFactory infoBarUIFactory = sp.GetService(typeof(SVsInfoBarUIFactory)) as IVsInfoBarUIFactory;
            if (infoBarUIFactory == null)
                return;

            var uiElement = infoBarUIFactory.CreateInfoBar(infoBar);
            InfoBarHost.AddInfoBar(uiElement);
            uiElement.Advise(this, out _cookie);
        }


        private IVsInfoBarHost InfoBarHost
        {
            get
            {
                if (_infoBarHost == null)
                {
                    var guid = new Guid(SharperCryptoApiAnalysisPane.SharperCryptoAnalysisPaneGuid);
                    var sp = VisualStudio.Integration.Services.SharperCryptoAnalysisServiceProvider;
                    var shell = (IVsUIShell)sp.GetService(typeof(SVsUIShell));
                    if (ErrorHandler.Failed(shell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref guid,
                        out var frame)))
                        return null;
                    if (ErrorHandler.Failed(frame.GetProperty((int)__VSFPROPID7.VSFPROPID_InfoBarHost, out var value)))
                        return null;
                    if (!(value is IVsInfoBarHost infoBarHost))
                        return null;
                    _infoBarHost = infoBarHost;
                }
                return _infoBarHost;
            }
        }

        private void OnSelectedFilterChanged()
        {
            CheckBoxesEnabled = SelectedFilter == ExtensionItemFilter.Update &&
                                ConfigurationManager.Configuration.SyncMode != ConfigSyncMode.Hard;

            UpdateItems();
        }

        private void UpdateItems()
        {
            JoinableTaskFactory.RunAsync(async () =>
                {
                    await JoinableTaskFactory.SwitchToMainThreadAsync();
                    await UpdateItemsAsync();
                });
        }

        private async Task UpdateItemsAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            SetLoadingState("Loading");
            ItemsSource.Clear();

            var readOnly = ConfigurationManager.Configuration.SyncMode == ConfigSyncMode.Hard;

            if (SelectedFilter == ExtensionItemFilter.Update)
            {
                await ConfigurationManager.LoadConfiguration();
                var updateMetadata =
                    ConfigurationManager.ExtensionManager.CheckForUpdates(CheckUpdateOption
                        .OnlyInstalledExtensions);
                if (!updateMetadata.Actions.Any())
                {
                    SetNotItemsFoundState();
                    return;
                }
                foreach (var entry in updateMetadata.Actions)
                    ItemsSource.Add(new ExtensionItemListViewModel(entry, readOnly));          
            }

            if (SelectedFilter == ExtensionItemFilter.Installed)
            {
                var installed = ConfigurationManager.ExtensionManager.InstalledExtensions;

                if (!installed.Any())
                {
                    SetNotItemsFoundState();
                    return;
                }

                foreach (var entry in installed)
                {
                    var latestVersion = ConfigurationManager.ExtensionManager.GetLatestAvailableVersion(entry);
                    ItemsSource.Add(new ExtensionItemListViewModel(entry, entry.Version, latestVersion, readOnly));
                }

            }

            if (SelectedFilter == ExtensionItemFilter.Available)
            {
                var availableExtensions = ConfigurationManager.Configuration.Extensions;

                foreach (var entry in availableExtensions)
                {
                    ConfigurationManager.ExtensionManager.IsExtensionInstalled(entry, out var currentVersion);
                    ItemsSource.Add(new ExtensionItemListViewModel(entry, currentVersion, entry.Version, readOnly));
                }
            }

            

            ReleaseLoadingState();

            SelectedItem = ItemsSource.OfType<IExtensionItemListViewModel>().FirstOrDefault();
        }

        private void SetNotItemsFoundState()
        {
            ItemsSource.Clear();
            _loadingStatusIndicator.Status = LoadingStatus.NoItemsFound;
            ItemsSource.Add(_loadingStatusIndicator);
            OnPropertyChanged(nameof(SelectedItem));
        }

        private void SetLoadingState(string message)
        {
            StatusText = message;
            ItemsSource.Clear();
            _loadingStatusIndicator.Reset(message);
            _loadingStatusIndicator.Status = LoadingStatus.Loading;
            ItemsSource.Add(_loadingStatusIndicator);
        }

        private void ReleaseLoadingState()
        {
            _loadingStatusIndicator.Reset(string.Empty);
            StatusText = "Ready";
        }

        private async void ExecuteUpdate()
        {
            StatusText = "Updating";

            try
            {
                var actions = ItemsSource.OfType<ExtensionItemListViewModel>().Where(x => x.Selected).Select(x => x.ToActionDataEntry()).ToList();

                if (!actions.Any())
                    return;

                var data = new ExtensionCheckActionData();
                foreach (var entry in actions)
                    data.AddAction(entry);
                await ConfigurationManager.ExtensionManager.PerformUpdate(data);
            }
            finally
            {
                StatusText = "Ready";
            }
        }

        private async void ExecuteSync()
        {
            StatusText = "Updating";

            try
            {
                SetLoadingState("Syncing");
                await ConfigurationManager.Sync();
            }
            finally
            {
                ReleaseLoadingState();
                await UpdateItemsAsync();
            }
        }

        private void ChangeSelectionOnAllItems(bool select)
        {
            foreach (var viewModel in ItemsSource.OfType<ExtensionItemListViewModel>())
                viewModel.Selected = select;
        }

        public void OnClosed(IVsInfoBarUIElement infoBarUIElement)
        {
            infoBarUIElement.Unadvise(_cookie);
            _currentInfoBar = null;
        }

        public void OnActionItemClicked(IVsInfoBarUIElement infoBarUIElement, IVsInfoBarActionItem actionItem)
        {
            if (!actionItem.Text.Equals("Restart"))
                return;
            var vs = VisualStudio.Integration.VisualStudio.GetInstance();
            vs.Restart();
        }
    }
}