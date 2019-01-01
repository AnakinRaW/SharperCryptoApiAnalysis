using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Shell.Commands;
using SharperCryptoApiAnalysis.Shell.Interop.Commands;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;
using Task = System.Threading.Tasks.Task;
using WizardWindow = SharperCryptoApiAnalysis.Shell.Interop.Wizard.WizardWindow;

namespace SharperCryptoApiAnalysis.Vsix.Commands
{
    [Export(typeof(IAddCryptoTaskCommand))]
    internal class AddCryptoTaskCommand : VsCommand, IAddCryptoTaskCommand
    {
        public const int CommandId = PackageIds.AddCryptoAbstractionFile;
        public static readonly Guid CommandSet = PackageGuids.SharperCryptoApiAnalysisCommandSet;
        private ISharperCryptoAnalysisServiceProvider _serviceProvider;
        private ICryptoCodeGenerationTaskProvider _cryptoCodeGenerationTaskProvider;

        private ICryptoCodeGenerationTaskProvider CryptoCodeGenerationTaskProvider => _cryptoCodeGenerationTaskProvider ??
                                                         (_cryptoCodeGenerationTaskProvider = _serviceProvider.ExportProvider
                                                             .GetExportedValue<ICryptoCodeGenerationTaskProvider>());

        [ImportingConstructor]
        public AddCryptoTaskCommand(ISharperCryptoAnalysisServiceProvider serviceProvider) : base(CommandSet, CommandId)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task Execute()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var viewModel = new CryptoTaskWizardViewModel(CryptoCodeGenerationTaskProvider);
            var w = new WizardWindow(viewModel);
            w.ShowDialog();

            if (!viewModel.Completed)
                return;

            var result = viewModel.Result as ICryptoCodeGenerationTask;
            result?.TaskHandler.Execute(result.Model);
        }
    }
}
