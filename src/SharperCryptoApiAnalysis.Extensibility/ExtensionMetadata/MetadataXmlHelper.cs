using System.Collections.Generic;
using System.Xml;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Extensibility.ExtensionMetadata
{
    public static class MetadataXmlHelper
    {
        public static IEnumerable<ISharperCryptoApiExtensionMetadata> GetExtensions(in XmlDocument document, string xPath)
        {
            var extensionNodes = document.SelectNodes(xPath);

            if (extensionNodes == null)
                return new List<ISharperCryptoApiExtensionMetadata>();

            var extensionsList = new List<ISharperCryptoApiExtensionMetadata>();
            foreach (XmlNode extensionNode in extensionNodes)
            {
                var metadata = SharperCryptoApiExtensionMetadata.FromXmlNode(extensionNode);
                extensionsList.Add(metadata);
            }

            return extensionsList;
        }
    }
}
