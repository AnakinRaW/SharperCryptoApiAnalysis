using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using JetBrains.Annotations;
using SharperCryptoApiAnalysis.Extensibility.ExtensionMetadata;
using SharperCryptoApiAnalysis.Extensibility.Utilities;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Installer.Win
{
    internal class PrivateSharperCryptoApiExtensionMetadata : IPrivateSharperCryptoApiExtensionMetadata,
        IEquatable<IPrivateSharperCryptoApiExtensionMetadata>, INotifyPropertyChanged
    {
        private bool _installAnalyzer;
        private bool _isAnalyzer;
        private bool _isMefComponent;
        private bool _isAssembly;

        public ExtensionType Type
        {
            get
            {
                ExtensionType types = ExtensionType.UnknownAssembly;

                if (IsAnalyzer)
                {
                    types &= ~ExtensionType.UnknownAssembly;
                    types |= ExtensionType.Analyzer;
                }

                if (IsMefComponent)
                {
                    types &= ~ExtensionType.UnknownAssembly;
                    types |= ExtensionType.MefComponent;
                }

                if (IsAssembly)
                {
                    types &= ~ExtensionType.UnknownAssembly;
                    types |= ExtensionType.Assembly;
                }

                return types;
            }
        }

        public string Name { get; }
        public string InstallPath { get; protected set; }

        public bool InstallExtension
        {
            get => _installAnalyzer;
            set
            {
                if (value == _installAnalyzer)
                {
                    return;
                }

                _installAnalyzer = value;
                OnPropertyChanged();
            }
        }

        public string Summary { get; }
        public string Description { get; }
        public string Source { get; protected set; }
        public bool External { get; }
        public ExtensionFileType FileType { get; }

        public Version Version { get; }

        public bool IsAnalyzer
        {
            get => _isAnalyzer;
            set
            {
                if (value == _isAnalyzer) return;
                _isAnalyzer = value;
                OnPropertyChanged();
            }
        }

        public bool IsMefComponent
        {
            get => _isMefComponent;
            set
            {
                if (value == _isMefComponent) return;
                _isMefComponent = value;
                OnPropertyChanged();
            }
        }

        public bool IsAssembly
        {
            get => _isAssembly;
            set
            {
                if (value == _isAssembly) return;
                _isAssembly = value;
                OnPropertyChanged();
            }
        }


        public string Author { get; }

        public PrivateSharperCryptoApiExtensionMetadata(IPrivateSharperCryptoApiExtensionMetadata extension)
        {
            Name = extension.Name;
            Version = extension.Version;
            Author = extension.Author;
            Summary = extension.Summary;
            External = extension.External;
            InstallExtension = extension.InstallExtension;
            Description = extension.Description;
            FileType = extension.FileType;
            IsAssembly = extension.IsAssembly;
            IsAnalyzer = extension.IsAnalyzer;
            IsMefComponent = extension.IsMefComponent;
            Source = extension.Source;
            InstallPath = extension.InstallPath;
        }

        public PrivateSharperCryptoApiExtensionMetadata(string filePath)
        {
            Name = Path.GetFileName(filePath);
            var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
            Version = Version.Parse(versionInfo.FileVersion);
            Author = versionInfo.CompanyName;
            Summary = versionInfo.FileDescription;
            External = false;
            InstallExtension = true;
            Description = string.Empty;

            var extension = Path.GetExtension(filePath);
            if (extension.EndsWith(".dll"))
                FileType = ExtensionFileType.Assembly;
            else if (extension.EndsWith(".zip"))
                FileType = ExtensionFileType.Assembly;
            else
                throw new NotSupportedException("The file type is not supported");
        }

        public void SetPathValues(string localRepoPath)
        {
            Source = Path.Combine(localRepoPath, Name);
            InstallPath = Path.Combine(Core.Constants.ExtensionsInstallDirectoryName, Name);
        }

        public bool Equals(IPrivateSharperCryptoApiExtensionMetadata other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Name, other.Name) && Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((IPrivateSharperCryptoApiExtensionMetadata)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
                return hashCode;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual XmlNode ToXmlNode(XmlDocument document)
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

    internal class LocalSharperCryptoApiExtensionMetadata : PrivateSharperCryptoApiExtensionMetadata, ILocalSharperCryptoApiExtensionMetadata,
        IEquatable<ILocalSharperCryptoApiExtensionMetadata>
    {
        private string _localPath;

        public string LocalPath
        {
            get => _localPath;
            set
            {
                if (value == _localPath) return;
                _localPath = value;
                OnPropertyChanged();
            }
        }

        public LocalSharperCryptoApiExtensionMetadata(string filePath) : base(filePath)
        {
            LocalPath = filePath;
        }

        public bool Equals(ILocalSharperCryptoApiExtensionMetadata other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(Name, other.Name) && string.Equals(LocalPath, other.LocalPath) &&
                   Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ILocalSharperCryptoApiExtensionMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LocalPath != null ? LocalPath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override XmlNode ToXmlNode(XmlDocument document)
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
            var localPath = XmlUtilities.CreateNamedElement(nameof(LocalPath), LocalPath, document);
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
            node.AppendChild(localPath);

            return node;
        }
    }
}