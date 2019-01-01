using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SharperCryptoApiAnalysis.Interop.Services;

namespace SharperCryptoApiAnalysis.VisualStudio.Integration
{
    /// <summary>
    /// Commonly used services in Sharper Crypto-API Analysis
    /// </summary>
    public static class Services
    {
        internal static IServiceProvider UnitTestServiceProvider { get; set; }

        /// <summary>
        /// The Visual Studio Shell
        /// </summary>
        public static IVsUIShell VsUiShell => GetGlobalService<SVsUIShell, IVsUIShell>();

        /// <summary>
        /// The Visual Studio DTE
        /// </summary>
        public static DTE Dte => GetGlobalService<DTE, DTE>();

        /// <summary>
        /// The Sharper Crypto-API Analysis Service Provider
        /// </summary>
        public static ISharperCryptoAnalysisServiceProvider SharperCryptoAnalysisServiceProvider =>
            GetGlobalService<ISharperCryptoAnalysisServiceProvider, ISharperCryptoAnalysisServiceProvider>();

        private static TV GetGlobalService<T, TV>(IServiceProvider provider = null) where T : class where TV : class
        {
            TV ret = null;
            if (provider != null)
                ret = provider.GetService(typeof(T)) as TV;
            if (ret != null)
                return ret;
            if (UnitTestServiceProvider != null)
                return UnitTestServiceProvider.GetService(typeof(T)) as TV;
            return Package.GetGlobalService(typeof(T)) as TV;
        }
    }
}
