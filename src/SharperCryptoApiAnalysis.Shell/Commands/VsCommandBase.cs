using System;
using System.ComponentModel.Design;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Shell.Interop.Commands;

namespace SharperCryptoApiAnalysis.Shell.Commands
{
    public abstract class VsCommandBase : OleMenuCommand, IVsCommandBase
    {
        private EventHandler _canExecuteChanged;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => _canExecuteChanged += value;
            remove => _canExecuteChanged -= value;
        }

        protected VsCommandBase(Guid commandSet, int commandId) : 
            base(InvokeHandler, delegate { }, QueryStatusHandler, new CommandID(commandSet, commandId))
        {

        }

        public bool CanExecute(object parameter)
        {
            QueryStatus();
            return Enabled && Visible;
        }

        public void Execute(object parameter)
        {
            ExecuteUntyped(parameter);
        }


        protected abstract void ExecuteUntyped(object parameter);

        protected virtual void QueryStatus()
        {
        }

        protected override void OnCommandChanged(EventArgs e)
        {
            base.OnCommandChanged(e);
            _canExecuteChanged?.Invoke(this, e);
        }

        private static void InvokeHandler(object sender, EventArgs e)
        {
            var args = (OleMenuCmdEventArgs) e;
            var command = sender as VsCommandBase;
            command?.ExecuteUntyped(args.InValue);
        }

        private static void QueryStatusHandler(object sender, EventArgs e)
        {
            var command = sender as VsCommandBase;
            command?.QueryStatus();
        }
    }
}
