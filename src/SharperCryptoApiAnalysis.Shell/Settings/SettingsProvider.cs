using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Interop.Settings;

namespace SharperCryptoApiAnalysis.Shell.Settings
{
    [Export(typeof(ISettingsProvider))]
    internal class SettingsProvider : ISettingsProvider
    {
        public ISharperCryptoApiAnalysisSettings Settings { get; }

        [ImportingConstructor]
        public SettingsProvider(ISharperCryptoAnalysisServiceProvider serviceProvider)
        {
            var cm = serviceProvider.GetService<ISharperCryptoApiAnalysisSettings>();
            Settings = cm;
        }
    }
}