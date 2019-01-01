using System;
using System.ComponentModel;
using System.Windows;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Interop.Settings;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.ViewModels;

namespace SharperCryptoApiAnalysis.Vsix.Views.Dialog
{
    public partial class StartupWindow
    {
        private ISharperCryptoApiAnalysisSettings _settings;
        private readonly IServiceProvider _serviceProvider;

        public StartupWindow()
        {
            InitializeComponent();
        }

        public StartupWindow(ISharperCryptoApiAnalysisSettings settings, IServiceProvider serviceProvider)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _settings.ShowStartupWindow = !DoNotShowAgainCheckbox.IsChecked.Value;
            _settings.Save();
        }

        private async void OpenToolClick(object sender, RoutedEventArgs e)
        {
            var toolManager = _serviceProvider.GetService(typeof(IToolWindowManager)) as IToolWindowManager;
            var toolWindow = await toolManager?.ShowToolPane();
            toolWindow?.ShowView<IManageConnectionsViewModel>();
            Close();
        }
    }
}
