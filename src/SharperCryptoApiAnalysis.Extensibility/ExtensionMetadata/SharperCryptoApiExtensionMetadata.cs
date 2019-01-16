using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Extensibility.Exceptions;
using SharperCryptoApiAnalysis.Extensibility.Utilities;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Extensibility.ExtensionMetadata
{
    internal sealed class SharperCryptoApiExtensionMetadata : ISharperCryptoApiExtensionMetadata
    {

        //For some mysterious reason Properties need to have a setter for a textbox to apply the Text binding
        public ExtensionType Type { get; }
        public string Name { get; }
        public string InstallPath { get; }
        public bool InstallExtension { get; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public Version Version { get; }
        public string Source { get; }
        public string Author { get; }
        public bool External { get; }
        public ExtensionFileType FileType { get; }

        public SharperCryptoApiExtensionMetadata(string name, ExtensionType type, string installPath, bool installExtension, string summary, string author, string description,
            Version version, string source, bool external)
        {
            Name = name;
            EnsureExtensionType(type);
            Type = type; 
            InstallPath = installPath;
            InstallExtension = installExtension;
            Summary = summary;
            Version = version;
            Source = source;
            External = external;
            Author = author;
            Description = description;

            var extension = Path.GetExtension(installPath);
            if (extension == null)
                throw new ArgumentException("Install path is not a valid file");

            if (extension.EndsWith(".dll"))
                FileType = ExtensionFileType.Assembly;
            else if (extension.EndsWith(".zip"))
                FileType = ExtensionFileType.Assembly;
            else
                throw new NotSupportedException("The file type is not supported");

        }

        private void EnsureExtensionType(ExtensionType type)
        {
            //All types are .dll based and compatible
            if ((int) type < 32)
                return;
            //All type are .zip based and compatible
            if ((int) type == 32 || (int) type == 64 || (int) type == 96)
                return;

            if (type == ExtensionType.UnknownZip || type == ExtensionType.UnknownAssembly)
                return;

            throw new NotSupportedException("An invalid extension type was specified. Cannot mix .dll based types with .zip based");
        }

        public static ISharperCryptoApiExtensionMetadata FromFile(string filePath, string relativeBasePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            if (relativeBasePath == null)
                throw new ArgumentNullException(nameof(relativeBasePath));

            var installPath = PathUtilities.GetRelativePath(relativeBasePath, filePath);
            var name = Path.GetFileName(filePath);
            var fileExtension = Path.GetExtension(filePath);

            ExtensionType type;
            switch (fileExtension)
            {
                //.dll File
                case string s when s.Equals(Constants.SupportedExtensionFiles[0]):
                    type = ExtensionType.UnknownAssembly;
                    break;
                case string s when s.Equals(Constants.SupportedExtensionFiles[0]):
                    type = ExtensionType.UnknownZip;
                    break;
                default:
                    throw new NotSupportedException("the specified file is not supported to be an extension.");
            }

            //TODO: Not sure if this is a good idea but for now it somehow replaces some better chained/multiple configurations where this assembly is included.
            if (name.Equals(Constants.DefaultAnalyzerAssemblyName))
                type = ExtensionType.Assembly | ExtensionType.Analyzer | ExtensionType.MefComponent;

            string summary;
            Version version;
            if (type == ExtensionType.UnknownAssembly)
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
                version = Version.Parse("1.0.0.0");
                summary = versionInfo.FileDescription;
            }
            else
            {
                summary = string.Empty;
                version = new Version("1.0.0.0");
            }

            return new SharperCryptoApiExtensionMetadata(name, type, installPath, true, summary, string.Empty, string.Empty, version, string.Empty, false);
        }

        public static ISharperCryptoApiExtensionMetadata FromXmlNode(in XmlNode node)
        {
            if (XmlUtilities.GetValueFromInnerText<string>(node, nameof(Name), out var name) != GetValueResult.Success)
                throw new MetadataParseException();
            if (XmlUtilities.GetValueFromInnerText<string>(node, nameof(InstallPath), out var installPath) != GetValueResult.Success)
                throw new MetadataParseException();
            if (XmlUtilities.GetValueFromInnerText<string>(node, nameof(Summary), out var summary) !=
                GetValueResult.Success)
                summary = string.Empty;
            if (XmlUtilities.GetValueFromInnerText<string>(node, nameof(Source), out var source) != GetValueResult.Success)
                throw new MetadataParseException();
            if (XmlUtilities.GetValueFromInnerText<string>(node, nameof(Version), out var version) != GetValueResult.Success)
                throw new MetadataParseException();

            if (XmlUtilities.GetValueFromInnerText<string>(node, nameof(Author), out var author) != GetValueResult.Success)
                author = string.Empty;
            if (XmlUtilities.GetValueFromInnerText<string>(node, nameof(Description), out var description) != GetValueResult.Success)
                description = string.Empty;

            if (XmlUtilities.GetValueFromInnerText<bool>(node, nameof(InstallExtension), out var installExtension) != GetValueResult.Success)
                throw new MetadataParseException();
            if (XmlUtilities.GetValueFromInnerText<bool>(node, nameof(External), out var external) != GetValueResult.Success)
                throw new MetadataParseException();


            if (XmlUtilities.GetValueFromInnerText<string>(node, nameof(Type), out var typeString) != GetValueResult.Success)
                throw new MetadataParseException();
            var type = (ExtensionType) Enum.Parse(typeof(ExtensionType), typeString);
            return new SharperCryptoApiExtensionMetadata(name, type, installPath, installExtension, summary, author, description, Version.Parse(version), source, external);
        }

        public bool Equals(ISharperCryptoApiExtensionMetadata other)
        {
            return string.Equals(Name, other.Name) && string.Equals(InstallPath, other.InstallPath) && Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ISharperCryptoApiExtensionMetadata other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (InstallPath != null ? InstallPath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
                return hashCode;
            }
        }

        public XmlNode ToXmlNode(XmlDocument document)
        {
            var node = document.CreateElement(MetadataConstants.ExtensionMetadataXmlNodeName);

            var name = XmlUtilities.CreateNamedElement(nameof(Name), Name, document);
            var author = XmlUtilities.CreateNamedElement(nameof(Author), Author, document);
            var version = XmlUtilities.CreateNamedElement(nameof(Version), Version, document);
            var summary = XmlUtilities.CreateNamedElement(nameof(Summary), Summary, document);
            var description = XmlUtilities.CreateNamedElement(nameof(Description), Description, document);
            var installPath = XmlUtilities.CreateNamedElement(nameof(InstallPath), InstallPath, document);
            var install = XmlUtilities.CreateNamedElement(nameof(InstallExtension), InstallExtension, document);
            var external = XmlUtilities.CreateNamedElement(nameof(External), External, document);
            var source = XmlUtilities.CreateNamedElement(nameof(Source), Source, document);
            var type = XmlUtilities.CreateNamedElement(nameof(Type), Type.ToString(), document);

            node.AppendChild(name);
            node.AppendChild(type);
            node.AppendChild(author);
            node.AppendChild(version);
            node.AppendChild(summary);
            node.AppendChild(description);
            node.AppendChild(installPath);
            node.AppendChild(install);
            node.AppendChild(external);
            node.AppendChild(source);

            return node;
        }
    }
}