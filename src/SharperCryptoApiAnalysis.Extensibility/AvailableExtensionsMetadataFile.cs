using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using SharperCryptoApiAnalysis.Extensibility.ExtensionMetadata;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Extensibility
{
    internal static class AvailableExtensionsMetadataFile
    {
        public const string FileName = "AvailableExtensions.metadata";

        public const string RootNode = "Extensions";

        public static AvailableExtensionsList OpenCreate(string filePath, IEnumerable<ISharperCryptoApiExtensionMetadata> availableExtensions)
        {
            using (var fs = new FileStream(Path.Combine(filePath, FileName), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                if (fs.Length == 0)
                {
                    var localExtensions = new AvailableExtensionsList();
                    foreach (var extension in availableExtensions)
                        localExtensions.Add(extension);

                    return localExtensions;
                }

                using (var tr = new XmlTextReader(fs))
                {
                    var doc = new XmlDocument();
                    tr.Namespaces = false;
                    doc.Load(tr);
                    return FromXml(doc, availableExtensions.ToList());
                }
            }
        }

        private static AvailableExtensionsList FromXml(XmlDocument document, IReadOnlyCollection<ISharperCryptoApiExtensionMetadata> availableExtensions)
        {
            var extensions = MetadataXmlHelper.GetExtensions(document,
                $"{RootNode}/{MetadataConstants.ExtensionMetadataXmlNodeName}");

            var localExtensions = new AvailableExtensionsList();

            foreach (var extension in availableExtensions)
                localExtensions.Add(extension);

            foreach (var extension in extensions)
            {
                if (availableExtensions.Any(x => x.Equals(extension)) || extension.External)
                    localExtensions.Add(extension);

                //If there is a version mismatch between current and latest version we still want to keep the source property
                var t = localExtensions.FirstOrDefault(x => x.Name.Equals(extension.Name) && x.InstallPath.Equals(extension.InstallPath));
                if (t != null)
                {
                    //TODO: Special handle .zip files as the file does not have a file verison attribute. 

                    var index = localExtensions.IndexOf(t);
                    var update = new SharperCryptoApiExtensionMetadata(t.Name, extension.Type, t.InstallPath, t.InstallExtension,
                        t.Summary, extension.Author, extension.Description, t.Version, extension.Source, t.External);
                    localExtensions.Update(index , update);
                }
            }

            return localExtensions;
        }

        public static void Save(IEnumerable<ISharperCryptoApiExtensionMetadata> extensionsCache, string filePath)
        {
            using (var fs = new FileStream(Path.Combine(filePath, FileName), FileMode.Create, FileAccess.Write))
            {
                var xmlDocument = new XmlDocument();
                var extensionsNode = xmlDocument.CreateElement(RootNode);
                foreach (var extension in extensionsCache)
                    extensionsNode.AppendChild(extension.ToXmlNode(xmlDocument));

                xmlDocument.AppendChild(extensionsNode);
                fs.Seek(0, SeekOrigin.Begin);
                xmlDocument.Save(fs);
            }
        }
    }
}