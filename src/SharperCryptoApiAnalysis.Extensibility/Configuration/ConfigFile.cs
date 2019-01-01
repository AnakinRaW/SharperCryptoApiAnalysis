using System.Collections.Generic;
using System.IO;
using System.Xml;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Configuration;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Extensibility.Configuration
{
    public abstract class ConfigFile<T> where T : ISharperCryptoApiExtensionMetadata
    {
        public const string RootNodeName = "SharperCryptoAPIConfiguration";
        public const string RepoAddressNodeName = "RepoAddress";
        public const string SyncModeNodeName = "SyncMode";
        public const string ExtensionsListNodeName = "Extensions";

        public ConfigSyncMode SyncMode { get; }

        public string RepoAddress { get; }

        public IEnumerable<T> Extensions { get; }

        public abstract string FileName { get; }

        public AnalysisSeverity AnalysisSeverity { get; }

        protected ConfigFile(string repoAddress, ConfigSyncMode mode, AnalysisSeverity severity, IEnumerable<T> extensions)
        {
            RepoAddress = repoAddress;
            SyncMode = mode;
            Extensions = extensions;
            AnalysisSeverity = severity;
        }

        public void WriteToFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var xmlDocument = new XmlDocument();

                var root = xmlDocument.CreateElement(string.Empty, ConfigFileXmlHelper.RootNodeName, string.Empty);
                xmlDocument.AppendChild(root);


                ConfigFileXmlHelper.CreateConfigElements(ref root, SyncMode, RepoAddress, AnalysisSeverity);

                var extensionList = xmlDocument.CreateElement(ConfigFileXmlHelper.ExtensionsListNodeName);
                foreach (var analyzer in Extensions)
                {
                    var analyzerNode = analyzer.ToXmlNode(xmlDocument);
                    extensionList.AppendChild(analyzerNode);
                }
                root.AppendChild(extensionList);

                fs.Seek(0, SeekOrigin.Begin);
                xmlDocument.Save(fs);
            }
        }

        public string GenerateFilePath(string directoryPath)
        {
            return Path.Combine(directoryPath, FileName);
        }
    }
}