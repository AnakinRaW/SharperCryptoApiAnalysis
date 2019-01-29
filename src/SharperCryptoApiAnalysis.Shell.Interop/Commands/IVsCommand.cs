using System.Threading.Tasks;

namespace SharperCryptoApiAnalysis.Shell.Interop.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <seealso cref="T:SharperCryptoApiAnalysis.Shell.Interop.Commands.IVsCommandBase" />
    public interface IVsCommand : IVsCommandBase
    {
        Task Execute();
    }
}
