using System.Collections.Generic;
using SharperCryptoApiAnalysis.Extensibility.Configuration;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Configuration;

namespace SharperCryptoApiAnalysis.Installer.Win
{
    public class PrivateConfigFile : ConfigFile<IPrivateSharperCryptoApiExtensionMetadata>
    {
        public PrivateConfigFile(string repoAddress, ConfigSyncMode mode, AnalysisSeverity severity, IReadOnlyCollection<IPrivateSharperCryptoApiExtensionMetadata> extensions) : 
            base(repoAddress, mode, severity, extensions)
        {
        }

        public override string FileName => "SharperCryptoApiAnalyzer.private.config";
    }
}