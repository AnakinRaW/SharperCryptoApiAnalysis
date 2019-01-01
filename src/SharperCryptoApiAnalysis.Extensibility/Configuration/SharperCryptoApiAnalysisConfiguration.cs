using System.Collections.Generic;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Configuration;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Extensibility.Configuration
{
    public class SharperCryptoApiAnalysisConfiguration : ISharperCryptoApiAnalysisConfiguration
    {
        public ConfigSyncMode SyncMode { get; }
        public AnalysisSeverity AnalysisSeverity { get; }

        public string RepoAddress { get; }

        public IEnumerable<ISharperCryptoApiExtensionMetadata> Extensions { get; }

        public SharperCryptoApiAnalysisConfiguration(string repoAddress, ConfigSyncMode mode, AnalysisSeverity severity, IEnumerable<ISharperCryptoApiExtensionMetadata> extensions)
        {
            SyncMode = mode;
            RepoAddress = repoAddress;
            Extensions = extensions;
            AnalysisSeverity = severity;
        }

        public static SharperCryptoApiAnalysisConfiguration FromFile(PublicConfigFile configFile)
        {
            return new SharperCryptoApiAnalysisConfiguration(configFile.RepoAddress, configFile.SyncMode, configFile.AnalysisSeverity, configFile.Extensions);
        }
    }
}