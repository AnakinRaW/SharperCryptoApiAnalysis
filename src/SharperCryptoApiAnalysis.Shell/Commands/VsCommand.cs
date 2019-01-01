using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using SharperCryptoApiAnalysis.Shell.Interop.Commands;

namespace SharperCryptoApiAnalysis.Shell.Commands
{
    public abstract class VsCommand : VsCommandBase, IVsCommand
    {
        protected VsCommand(Guid commandSet, int commandId) : base(commandSet, commandId)
        {
        }

        public abstract Task Execute();

        protected override void ExecuteUntyped(object parameter)
        {
            Execute().Forget();
        }
    }
}
