using System;
using System.ComponentModel.Design;
using System.Windows.Input;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Shell.Interop.Commands;

namespace SharperCryptoApiAnalysis.Shell.Commands
{
    public static class MenuCommandServiceExtensions
    {
        public static void AddCommands(this IMenuCommandService service, params IVsCommandBase[] commands)
        {
            Validate.IsNotNull(service, nameof(service));
            Validate.IsNotNull(commands, nameof(commands));

            foreach (MenuCommand command in commands)
            {
                service.AddCommand(command);
            }
        }

        public static OleMenuCommand BindCommand(
            this IMenuCommandService service,
            CommandID id,
            ICommand command,
            bool hideWhenDisabled = false)
        {
            Validate.IsNotNull(service, nameof(service));
            Validate.IsNotNull(id, nameof(id));
            Validate.IsNotNull(command, nameof(command));

            var bound = new BoundCommand(id, command, hideWhenDisabled);
            service.AddCommand(bound);
            return bound;
        }

        private class BoundCommand : OleMenuCommand
        {
            private readonly ICommand _inner;
            private readonly bool _hideWhenDisabled;

            public BoundCommand(CommandID id, ICommand command, bool hideWhenDisabled)
                : base(InvokeHandler, delegate { }, HandleBeforeQueryStatus, id)
            {
                Validate.IsNotNull(id, nameof(id));
                Validate.IsNotNull(command, nameof(command));

                _inner = command;
                _hideWhenDisabled = hideWhenDisabled;
                _inner.CanExecuteChanged += (s, e) => HandleBeforeQueryStatus(this, e);
            }

            private static void InvokeHandler(object sender, EventArgs e)
            {
                var command = sender as BoundCommand;
                command?._inner.Execute((e as OleMenuCmdEventArgs)?.InValue);
            }

            private static void HandleBeforeQueryStatus(object sender, EventArgs e)
            {
                if (sender is BoundCommand command)
                {
                    command.Enabled = command._inner.CanExecute(null);
                    command.Visible = !command._hideWhenDisabled || command.Enabled;
                }
            }
        }
    }
}
