using System;
using System.Xml;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Configuration;

namespace SharperCryptoApiAnalysis.Extensibility.Configuration
{
    internal static class ConfigFileXmlHelper
    {
        public const string RootNodeName = "SharperCryptoAPIConfiguration";
        public const string RepoAddressNodeName = "RepoAddress";
        public const string AnalysisSeverityNodeName = "AnalysisSeverity";
        public const string SyncModeNodeName = "SyncMode";
        public const string SeverityNodeName = "AnalysisSeverity";
        public const string ExtensionsListNodeName = "Extensions";

        public static bool TryGetAddress(XmlDocument document, out string address)
        {
            address = string.Empty;
            var addressNode = document.SelectSingleNode($"{RootNodeName}/{RepoAddressNodeName}");
            if (addressNode == null)
                return false;
            address = addressNode.InnerText;
            return true;
        }

        public static bool TryGetSyncMode(XmlDocument document, out ConfigSyncMode syncMode)
        {
            syncMode = ConfigSyncMode.Undefined;
            var syncModeNode = document.SelectSingleNode($"{RootNodeName}/{SyncModeNodeName}");
            return syncModeNode != null && Enum.TryParse(syncModeNode.InnerText, out syncMode);
        }

        public static AnalysisSeverity GetSeverity(XmlDocument document)
        {
            var severityNode = document.SelectSingleNode($"{RootNodeName}/{SeverityNodeName}");
            if (severityNode == null || !Enum.TryParse(severityNode.InnerText, out AnalysisSeverity severity))
                return AnalysisSeverity.Default;
            return severity;
        }

        public static void CreateConfigElements(ref XmlElement rootNode, ConfigSyncMode syncMode, string repoAddress, AnalysisSeverity analysisSeverity)
        {
            var syncModeNode = rootNode.OwnerDocument.CreateElement(SyncModeNodeName);
            syncModeNode.InnerText = syncMode.ToString();

            var severityNode = rootNode.OwnerDocument.CreateElement(AnalysisSeverityNodeName);
            severityNode.InnerText = analysisSeverity.ToString();

            var repoAddressNode = rootNode.OwnerDocument.CreateElement(RepoAddressNodeName);
            repoAddressNode.InnerText = repoAddress;     

            rootNode.AppendChild(syncModeNode);
            rootNode.AppendChild(repoAddressNode);
            rootNode.AppendChild(severityNode);
        }
    }
}
