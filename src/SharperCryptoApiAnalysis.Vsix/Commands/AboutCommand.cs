using System;
using System.ComponentModel.Composition;
using Microsoft.Internal.VisualStudio.PlatformUI;
using SharperCryptoApiAnalysis.Shell.Commands;
using SharperCryptoApiAnalysis.Shell.Interop.Commands;
using SharperCryptoApiAnalysis.Vsix.Views.Dialog;
using Task = System.Threading.Tasks.Task;

namespace SharperCryptoApiAnalysis.Vsix.Commands
{
    [Export(typeof(IAboutCommand))]
    public class AboutCommand : VsCommand, IAboutCommand
    {
        private static readonly Guid CommandSet = new Guid(PackageGuids.SharperCryptoApiAnalysisCommandSetString);

        private const int CommandId = PackageIds.AboutCommand;

        public AboutCommand() : base(CommandSet, CommandId)
        {
        }

        public override Task Execute()
        {
            var dw = new AboutDialog();
            WindowHelper.ShowModal(dw);

            return Task.CompletedTask;
        }
    }
}
