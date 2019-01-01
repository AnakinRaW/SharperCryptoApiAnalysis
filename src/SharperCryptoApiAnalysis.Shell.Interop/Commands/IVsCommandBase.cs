using System.Windows.Input;

namespace SharperCryptoApiAnalysis.Shell.Interop.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// VS command base
    /// </summary>
    /// <seealso cref="T:System.Windows.Input.ICommand" />
    public interface IVsCommandBase : ICommand
    {
        /// <summary>
        /// Indicating whether this command is enabled.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if enabled; otherwise, <see langword="false"/>.
        /// </value>
        bool Enabled { get; }

        /// <summary>
        /// Indicating whether this command is visible.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if visible; otherwise, <see langword="false"/>.
        /// </value>
        bool Visible { get; }
    }
}
