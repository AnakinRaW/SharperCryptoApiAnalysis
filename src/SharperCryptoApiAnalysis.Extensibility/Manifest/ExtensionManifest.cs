using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SharperCryptoApiAnalysis.Extensibility.Manifest
{
    public class ExtensionManifest
    {
        private ICollection<Asset> _assets { get; }

        public string FilePath { get; private set; }

        public IReadOnlyCollection<Asset> Assets => _assets.ToList();

        private ExtensionManifest()
        {
            _assets = new HashSet<Asset>();
        }

        public static ExtensionManifest LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Manifest file does not exist");

            var manifest = new ExtensionManifest();

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var xmldoc = new XmlDocument();
                var ns = new XmlNamespaceManager(xmldoc.NameTable);
                ns.AddNamespace("msm", "http://schemas.microsoft.com/developer/vsx-schema/2011");
                xmldoc.Load(fs);
                var assets = xmldoc.SelectSingleNode("//msm:Assets", ns);
                if (assets == null)
                    throw new InvalidOperationException();

                foreach (XmlNode assetNode in assets.ChildNodes)
                {
                    var type = assetNode.Attributes["Type"];
                    var assetPath = assetNode.Attributes["Path"];
                    manifest._assets.Add(new Asset(type.InnerText, assetPath.InnerText));
                }
            }

            manifest.FilePath = path;
            return manifest;
        }

        public void SaveToFile()
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException();

            using (var fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var xmldoc = new XmlDocument();
                var ns = new XmlNamespaceManager(xmldoc.NameTable);
                ns.AddNamespace("msm", "http://schemas.microsoft.com/developer/vsx-schema/2011");
                xmldoc.Load(fs);
                var assets = xmldoc.SelectSingleNode("//msm:Assets", ns);
                if (assets == null)
                    throw new InvalidOperationException();

                assets.RemoveAll();
                WriteAssets(assets);

                fs.SetLength(0);
                xmldoc.Save(fs);
            }        
        }

        private void WriteAssets(in XmlNode rootNode)
        {
            foreach (var asset in Assets)
            {
                var assetNode = rootNode.OwnerDocument?.CreateElement("Asset", rootNode.OwnerDocument.DocumentElement.NamespaceURI);
                assetNode.SetAttribute("Type", null, AssetTypeHelper.TypeToString(asset.Type));
                assetNode.SetAttribute("Path", null, asset.FilePath);
                rootNode.AppendChild(assetNode);
            }
        }

        public void AddAsset(Asset asset)
        {
            if (!_assets.Contains(asset))
                _assets.Add(asset);
        }

        public void RemoveAsset(Asset asset)
        {
            if (Assets.Contains(asset))
                _assets.Remove(asset);
        }
    }
}