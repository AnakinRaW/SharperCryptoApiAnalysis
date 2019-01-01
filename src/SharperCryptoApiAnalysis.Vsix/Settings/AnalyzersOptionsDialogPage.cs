using System.ComponentModel;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Settings;

namespace SharperCryptoApiAnalysis.Vsix.Settings
{
    internal class AnalyzersOptionsDialogPage : UIElementDialogPage
    {
        private ISharperCryptoApiAnalysisSettings _settings;
        private IAnalyzerManager _analyzerManager;
        private AnalyzersOptionsDialogControl _dialogControl;

        private ISharperCryptoApiAnalysisSettings Settings =>
            _settings ?? (_settings = (ISharperCryptoApiAnalysisSettings) Site.GetService(
                typeof(ISharperCryptoApiAnalysisSettings)));

        private IAnalyzerManager AnalyzerManager =>
            _analyzerManager ?? (_analyzerManager = (IAnalyzerManager) Site.GetService(
                typeof(IAnalyzerManager)));

        protected override UIElement Child => _dialogControl ?? (_dialogControl = new AnalyzersOptionsDialogControl(AnalyzerManager));

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);
            _dialogControl.Analyzers = AnalyzerManager.Analyzers;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                foreach (var analyzer in _dialogControl.Analyzers.ToList())
                {
                    if (analyzer.IsMuted)
                        Settings.MutedAnalyzers.Add((int) analyzer.AnalyzerId);
                    else
                        Settings.MutedAnalyzers.Remove((int)analyzer.AnalyzerId);
                }
                    
                Settings.Save();
            }
            base.OnApply(e);
        }
    }
}
