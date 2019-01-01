using System.Threading.Tasks;

namespace SharperCryptoApiAnalysis.Shell.Interop.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Shell.Interop.Commands.IVsCommandBase" />
    public interface IVsCommand : IVsCommandBase
    {
        Task Execute();
    }
}
