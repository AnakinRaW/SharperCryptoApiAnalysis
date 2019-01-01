using System;
using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Interop.Services;
using SharperCryptoApiAnalysis.Shell;
using SharperCryptoApiAnalysis.Shell.Commands;
using SharperCryptoApiAnalysis.Shell.Interop.Commands;
using Task = System.Threading.Tasks.Task;

namespace SharperCryptoApiAnalysis.Vsix.Commands
{
    [Export(typeof(IShowSharperCryptoAnalysisPaneCommand))]
    internal sealed class SharperCryptoAnalysisPaneCommand : VsCommand, IShowSharperCryptoAnalysisPaneCommand
    {
        /// <summary>
        /// Gets the GUID of the group the command belongs to.
        /// </summary>
        public static readonly Guid CommandSet = PackageGuids.SharperCryptoApiAnalysisCommandSet;

        /// <summary>
        /// Gets the numeric identifier of the command.
        /// </summary>
        public const int CommandId = PackageIds.ShowPaneCommand;

        private readonly ISharperCryptoAnalysisServiceProvider _serviceProvider;

        [ImportingConstructor]
        public SharperCryptoAnalysisPaneCommand(ISharperCryptoAnalysisServiceProvider serviceProvider) : base(CommandSet, CommandId)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task Execute()
        {
            try
            {
                var host = await _serviceProvider.TryGetService<IToolWindowManager>().ShowToolPane();
            }
            catch
            {
            }
            
        }
    }
}
