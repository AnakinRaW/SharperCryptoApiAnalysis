namespace SharperCryptoApiAnalysis.Core
{
    public static class Constants
    {
        /// <summary>
        /// The file name of the configuration file in a git repository
        /// </summary>
        public const string ConfigurationFileName = "SharperCryptoApiAnalyzer.config";

        /// <summary>
        /// The sub installation directory name where extensions get installed
        /// </summary>
        public const string ExtensionsInstallDirectoryName = "Extensions";

        /// <summary>
        /// The VS extensions manifest file name
        /// </summary>
        public const string VsixManifestFileName = "extension.vsixmanifest";

        /// <summary>
        /// The file extensions of supported Sharper Crypto-API Analysis extensions
        /// </summary>
        public static readonly string[] SupportedExtensionFiles = { ".dll", ".zip" };

        /// <summary>
        /// The assembly name of the default extension 
        /// </summary>
        public const string DefaultAnalyzerAssemblyName = "SharperCryptoApiAnalysis.BaseAnalyzers";
    }
}
