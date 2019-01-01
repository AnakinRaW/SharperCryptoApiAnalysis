using System.Diagnostics;

namespace SharperCryptoApiAnalysis.VisualStudio.Integration
{
    public static class ProcessHelper
    {
        /// <summary>
        /// Checks if the current process is Visual Studio
        /// </summary>
        /// <returns><see langword="true"/> if current process is VS; <see langword="true"/> otherwise</returns>
        public static bool IsRunningInsideVisualStudio()
        {
            var p = Process.GetCurrentProcess().ProcessName;
            return p == "devenv";
        }
    }
}
