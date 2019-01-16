using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Settings;

namespace SharperCryptoApiAnalysis.Vsix.Settings
{
    internal class GeneralOptionsDialogPage : UIElementDialogPage
    {
        private ISharperCryptoApiAnalysisSettings _settings;
        private IAnalyzerManager _analyzerManager;
        private GeneralOptionsDialogControl _dialogControl;

        private ISharperCryptoApiAnalysisSettings Settings =>
            _settings ?? (_settings = (ISharperCryptoApiAnalysisSettings) Site.GetService(
                typeof(ISharperCryptoApiAnalysisSettings)));

        protected override UIElement Child => _dialogControl ?? (_dialogControl = new GeneralOptionsDialogControl(Settings));

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);
            _dialogControl.ShowStartupWindowCheckbox.IsChecked = Settings.ShowStartupWindow;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
                Settings.Save();
            base.OnApply(e);
        }
    }
}
