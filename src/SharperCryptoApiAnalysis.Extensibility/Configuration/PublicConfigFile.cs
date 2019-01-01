using System.Collections.Generic;
using System.Xml;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Extensibility.Exceptions;
using SharperCryptoApiAnalysis.Extensibility.ExtensionMetadata;
using SharperCryptoApiAnalysis.Extensibility.Utilities;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Configuration;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Extensibility.Configuration
{
    public class PublicConfigFile : ConfigFile<ISharperCryptoApiExtensionMetadata>
    {
        public PublicConfigFile(string repoAddress, ConfigSyncMode mode, AnalysisSeverity severity, IEnumerable<ISharperCryptoApiExtensionMetadata> extensions) :
            base(repoAddress, mode, severity, extensions)
        {
        }

        public override string FileName => Constants.ConfigurationFileName;

        public static PublicConfigFile LoadFromData(string data)
        {
            var stream = data.ToStream();
            var doc = new XmlDocument();
            using (stream)
            {
                using (var tr = new XmlTextReader(stream))
                {
                    tr.Namespaces = false;
                    doc.Load(tr);
                }
            }

            if (!doc.HasChildNodes)
                throw new ConfigFileParseException();

            if (!ConfigFileXmlHelper.TryGetAddress(doc, out var address))
                throw new ConfigFileParseException();
            if (!ConfigFileXmlHelper.TryGetSyncMode(doc, out var syncMode))
                throw new ConfigFileParseException();

            var severity = ConfigFileXmlHelper.GetSeverity(doc);

            var extensions = MetadataXmlHelper.GetExtensions(doc, $"{RootNodeName}/{ExtensionsListNodeName}/{MetadataConstants.ExtensionMetadataXmlNodeName}");
            return new PublicConfigFile(address, syncMode, severity, extensions);
        }
    }
}