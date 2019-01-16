using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Extensibility.ExtensionMetadata;
using SharperCryptoApiAnalysis.Extensibility.Manifest;
using SharperCryptoApiAnalysis.Extensibility.Utilities;
using SharperCryptoApiAnalysis.Interop.Configuration;
using SharperCryptoApiAnalysis.Interop.Extensibility;
using SharperCryptoApiAnalysis.Interop.Services;

namespace SharperCryptoApiAnalysis.Extensibility
{
    internal sealed class ExtensionManager : IExtensionManager, ITestExtensionManager
    {
        private readonly ObservableCollection<ISharperCryptoApiExtensionMetadata> _installedExtensions;
        private bool _canCheckUpdates;
        private bool _initialized;
        private string _installPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler UpdateCompleted;



        public IAvailableExtensionsCache AvailableExtensions { get; }

        public bool CanCheckUpdates
        {
            get => _canCheckUpdates;
            private set
            {
                if (_canCheckUpdates == value)
                    return;
                _canCheckUpdates = value;
                OnPropertyChanged();
            }
        }

        public IConfigurationManager ConfigurationManager { get; }

        public int ExtensionsCount => InstalledExtensions.Count;

        public ReadOnlyObservableCollection<ISharperCryptoApiExtensionMetadata> InstalledExtensions { get; }

        public string InstallPath
        {
            get => _installPath;
            private set
            {
                if (value == _installPath) return;

                _installPath = value;
                OnPropertyChanged();
            }
        }

        public IReadOnlyCollection<ISharperCryptoApiExtensionMetadata> LocalExtensions
        {
            get
            {
                //Very slow and expensive but works

                if (!_initialized)
                    throw new ArgumentException();

                var extensionPath = Path.Combine(InstallPath, Constants.ExtensionsInstallDirectoryName);
                if (!Directory.Exists(extensionPath))
                    return new List<ISharperCryptoApiExtensionMetadata>();

                var lookupDict = new Dictionary<string, ISharperCryptoApiExtensionMetadata>();

                var localExtensions = GetLocalExtensionsFromFiles(extensionPath);

                foreach (var localExtension in localExtensions)
                {
                    var extension = SharperCryptoApiExtensionMetadata.FromFile(localExtension, InstallPath);
                    lookupDict.Add(extension.Name, extension);
                }

                var availableExtensions = AvailableExtensionsMetadataFile.OpenCreate(extensionPath, lookupDict.Values);

                var realLocalExtensions = new List<ISharperCryptoApiExtensionMetadata>();

                foreach (var metadata in availableExtensions.ToList())
                {
                    if (metadata.FileType != ExtensionFileType.Assembly)
                    {
                        realLocalExtensions.Add(metadata);
                        continue;
                    }

                    var versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(InstallPath, metadata.InstallPath));
                    var version = Version.Parse(versionInfo.FileVersion);
                    var newMetadata = new SharperCryptoApiExtensionMetadata(metadata.Name, metadata.Type, metadata.InstallPath,
                        metadata.InstallExtension, metadata.Summary, metadata.Author, metadata.Description, version,
                        metadata.Source,
                        metadata.External);
                    realLocalExtensions.Add(newMetadata);
                }

                return realLocalExtensions;

                //return (from metadata in availableExtensions
                //        let filePath = Path.Combine(InstallPath, metadata.InstallPath)
                //        let versionInfo = FileVersionInfo.GetVersionInfo(filePath)
                //        let version = Version.Parse(versionInfo.FileVersion)
                //        select new SharperCryptoApiExtensionMetadata(metadata.Name, metadata.InstallPath,
                //            metadata.InstallExtension, metadata.Summary, metadata.Author, metadata.Description, version, metadata.Source,
                //            metadata.External)
                //    ).Cast<ISharperCryptoApiExtensionMetadata>().ToList();
            }
        }
        
        private VisualStudio.Integration.VisualStudio VisualStudio { get; }

        private ExtensionManifest ManifestFile { get; set; }


        internal string ExtensionInstallPath => Path.Combine(InstallPath, Constants.ExtensionsInstallDirectoryName);



        public ExtensionManager(IConfigurationManager configurationManager, string configurationSuffix)
        {
            ConfigurationManager = configurationManager;

            _installedExtensions = new ObservableCollection<ISharperCryptoApiExtensionMetadata>();
            InstalledExtensions =
                new ReadOnlyObservableCollection<ISharperCryptoApiExtensionMetadata>(_installedExtensions);

            AvailableExtensions = new AvailableExtensionsCache(ConfigurationManager);

            VisualStudio = SharperCryptoApiAnalysis.VisualStudio.Integration.VisualStudio.GetInstance(configurationSuffix);
            Initialize();

            ConfigurationManager.ConfigurationChanged += OnConfigurationChanged;
        }

        public ExtensionManager(IConfigurationManager configurationManager) : this(configurationManager, string.Empty)
        {
        }

        public ExtensionCheckActionData CheckForUnmanagedExtensions()
        {
            if (!ConfigurationManager.HasConfiguration)
                throw new NotSupportedException(
                    "Checking for updates without a loaded configuration is currently not supported");

            var installedExtensions = InstalledExtensions;
            var availableExtensions = AvailableExtensions;
            var localExtensions = LocalExtensions;

            var result = new ExtensionCheckActionData();

            //Get all currently installed extensions that should be uninstalled because they are not registered in the current configuration
            foreach (var metadata in installedExtensions.Where(x =>
                !availableExtensions.Extensions.Any(y => x.Name.Equals(y.Name) && x.InstallPath.Equals(y.InstallPath))))
            {
                if (metadata.Name.Equals(Constants.DefaultAnalyzerAssemblyName))
                    continue;
                var action = ConfigurationManager.Configuration.SyncMode == ConfigSyncMode.Hard
                    ? ExtensionAction.Remove
                    : ExtensionAction.Uninstall;
                var entry = new ExtensionCheckActionDataEntry(metadata, action);
                result.AddAction(entry);

            }


            //Get all local extensions that should be removed because they are not registered in the current configuration
            if (ConfigurationManager.Configuration.SyncMode == ConfigSyncMode.Hard)
            {
                var filesToDelete = localExtensions.Where(x =>
                    !AvailableExtensions.Extensions.Any(
                        y => x.Name.Equals(y.Name) && x.InstallPath.Equals(y.InstallPath)));

                foreach (var metadata in filesToDelete)
                    result.AddAction(new ExtensionCheckActionDataEntry(metadata, ExtensionAction.Remove));
            }

            return result;
        }

        public void InvalidateAvailableExtensions()
        {
            AvailableExtensions.Invalidate();
        }

        public Version GetLatestAvailableVersion(ISharperCryptoApiExtensionMetadata metadata)
        {
            var availableExtensions = AvailableExtensions.Extensions;

            var possibleExtension = availableExtensions.FirstOrDefault(x =>
                x.Name.Equals(metadata.Name) && x.InstallPath.Equals(metadata.InstallPath));
            return possibleExtension?.Version;
        }

        public ExtensionCheckActionData CheckForUpdates(CheckUpdateOption option)
        {
            if (!ConfigurationManager.HasConfiguration)
                throw new NotSupportedException(
                    "Checking for updates without a loaded configuration is currently not supported");

            var installedExtensions = InstalledExtensions;
            var availableExtensions = AvailableExtensions;
            var result = new ExtensionCheckActionData();


            foreach (var toInstall in availableExtensions.Extensions.Where(x => x.InstallExtension))
            {
                var installedExtension = installedExtensions.FirstOrDefault(x =>
                    x.Name == toInstall.Name && x.InstallPath == toInstall.InstallPath);

                if (installedExtension == null)
                {
                    if (option == CheckUpdateOption.AllAvailableExtensions)
                    {
                        if (LocalExtensions.Contains(toInstall))
                        {
                            if (!IsExtensionInstalled(toInstall, out _))
                                result.AddAction(new ExtensionCheckActionDataEntry(toInstall, ExtensionAction.Install));
                        }
                        else
                            result.AddAction(new ExtensionCheckActionDataEntry(toInstall, ExtensionAction.DownloadAndInstall));
                    }     
                }
                else
                {
                    if (toInstall.Version == installedExtension.Version)
                        continue; 

                    result.AddAction(new ExtensionCheckActionDataEntry(toInstall, ExtensionAction.DownloadAndInstall, installedExtension?.Version));
                }
            }

            return result;
        }


        //TODO: Create a new return type that indicates the state of the installation, e.g. Installed, Corrupted, Missing File.
        //Also we could provide some fixing methods based on this result
        public bool IsExtensionInstalled(ISharperCryptoApiExtensionMetadata extension, out Version installedVersion)
        {
            installedVersion = default;

            var filePath = Path.Combine(InstallPath, extension.InstallPath);
            if (!File.Exists(filePath))
                return false;

            var assets = ManifestFile.Assets.Where(x => x.FilePath.Equals(extension.InstallPath)).ToList();
            if (assets.Count <= 0)
                return false;


            var types = Enum.GetValues(typeof(ExtensionType)).OfType<ExtensionType>().Where(x => extension.Type.HasFlag(x)).ToList();

            var flag = true;
            foreach (var type in types)
            {
                if (!assets.Any(x => x.Type.ToString().Equals(type.ToString())))
                    flag = false;
            }

            if (extension.FileType != ExtensionFileType.Assembly)
                return flag;

            var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
            installedVersion = Version.Parse(versionInfo.FileVersion);
            return flag;
        }


        public async Task PerformUpdate(ExtensionCheckActionData updateData)
        {
            var installOnly = GetMetadataFromUpdateAction(updateData, ExtensionAction.Install);
            var downloadInstall = GetMetadataFromUpdateAction(updateData, ExtensionAction.DownloadAndInstall);
            var remove = GetMetadataFromUpdateAction(updateData, ExtensionAction.Remove);
            var uninstall = GetMetadataFromUpdateAction(updateData, ExtensionAction.Uninstall);


            foreach (var downloadExtension in downloadInstall)
            {
                var storagePath = Path.Combine(InstallPath, downloadExtension.InstallPath);

                if (downloadExtension.External)
                    await ConfigurationManager.ConnectionManager.DownloadExternalFile(new Uri(downloadExtension.Source),
                        storagePath);
                else
                    await ConfigurationManager.ConnectionManager.DownloadFile(downloadExtension.Source, storagePath);        
            }


            foreach (var removeExtension in remove)
                DeleteExtension(removeExtension);
            foreach (var installExtension in downloadInstall.Union(installOnly))
                InstallExtension(installExtension);
            foreach (var removeExtension in uninstall.Union(remove))
                UninstallExtension(removeExtension);

            ManifestFile.SaveToFile();
            UpdateInstalledExtensions();
            OnUpdateCompleted();
        }

        private void InstallExtension(ISharperCryptoApiExtensionMetadata extension)
        {
            if (IsExtensionInstalled(extension, out _))
                return;

            var types = Enum.GetValues(typeof(ExtensionType)).OfType<ExtensionType>().Where(x => extension.Type.HasFlag(x)).ToList();

            foreach (var type in types)
            {
                if (!Enum.TryParse(type.ToString(), false, out AssetType assetType))
                    continue;
                ManifestFile.AddAsset(new Asset(assetType, extension.InstallPath));
            }
        }

        private void UninstallExtension(ISharperCryptoApiExtensionMetadata extension)
        {
            if (!IsExtensionInstalled(extension, out _))
                return;

            var types = Enum.GetValues(typeof(ExtensionType)).OfType<ExtensionType>().Where(x => extension.Type.HasFlag(x)).ToList();

            foreach (var type in types)
            {
                if (!Enum.TryParse(type.ToString(), false, out AssetType assetType))
                    continue;
                ManifestFile.RemoveAsset(new Asset(assetType, extension.InstallPath));
            }
        }

        private void DeleteExtension(ISharperCryptoApiExtensionMetadata extension)
        {
            var path = Path.Combine(InstallPath, extension.InstallPath);
            if (!File.Exists(path))
                return;
            File.Delete(path);
        }

        private IReadOnlyList<ISharperCryptoApiExtensionMetadata> GetMetadataFromUpdateAction(ExtensionCheckActionData data, ExtensionAction action)
        {
            return data.Actions.Where(x => x.Action == action).Select(x => x.ExtensionMetadata).ToList();
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FindInstallPath()
        {
            if (!_initialized) throw new ArgumentException();

            var extensionsPath = Path.Combine(VisualStudio.UserLocalPath, @"Extensions\");


            const string extensionDllFile = "SharperCryptoApiAnalysis.dll";
            var fileLocation = Directory.GetFiles(extensionsPath, extensionDllFile, SearchOption.AllDirectories)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(fileLocation))
                throw new FileNotFoundException("Could not find Sharper Crypto-API Analysis extension");

            InstallPath = new FileInfo(fileLocation).DirectoryName;
        }

        private void Initialize()
        {
            _initialized = true;
            FindInstallPath();
            ManifestFile = ExtensionManifest.LoadFromFile(Path.Combine(InstallPath, Constants.VsixManifestFileName));

            if (!Directory.Exists(ExtensionInstallPath))
                Directory.CreateDirectory(ExtensionInstallPath);

            UpdateInstalledExtensions();
        }

        private void OnConfigurationChanged(object sender, EventArgs e)
        {
            if (!ConfigurationManager.HasConfiguration)
                CanCheckUpdates = false;

            else
                ReloadConfigurationData();
        }

        private void ReloadConfigurationData()
        {
            if (!ConfigurationManager.HasConfiguration)
                throw new NullReferenceException(nameof(ConfigurationManager.Configuration));

            CanCheckUpdates = true;

            var config = ConfigurationManager.Configuration;


            UpdateInstalledExtensions();

            var extensionPath = Path.Combine(InstallPath, Constants.ExtensionsInstallDirectoryName);
            AvailableExtensionsMetadataFile.Save(AvailableExtensions.Extensions, extensionPath);

            if (config.SyncMode == ConfigSyncMode.Hard)
            {
                ConfigurationManager.Sync();
            }
        }

        private void UpdateInstalledExtensions()
        {
            if (!_initialized)
                throw new ArgumentException();

            AvailableExtensions.Invalidate();

            _installedExtensions.Clear();
            foreach (var metadata in LocalExtensions)
                if (IsExtensionInstalled(metadata, out _))
                    _installedExtensions.Add(metadata);
        }


        private IEnumerable<string> GetLocalExtensionsFromFiles(string extensionPath)
        {
            var excludeSubDirectories = new[] {"x64", "x86"};
            var supportedFileExtensions = new[] {".dll", ".zip"};
            var allFiles = Directory.EnumerateFiles(extensionPath, "*.*", SearchOption.AllDirectories).ToList();

            var relativePath = PathUtilities.GetRelativePath(VisualStudio.UserLocalPath, extensionPath);
            var relativePathLength = relativePath.Length;

            var foundFiles = new List<string>();


            foreach (var file in allFiles)
            {
                if (supportedFileExtensions.Any(x => x.Equals(Path.GetExtension(file))))
                {
                    var i = file.IndexOf(relativePath);
                    var shortPath = file.Substring(i + relativePathLength);

                    if (!excludeSubDirectories.Any(x => PathUtilities.ContainsSubPath(shortPath, $"\\{x}\\")))
                        foundFiles.Add(file);
                }
            }

            return foundFiles;
        }

        public IEnumerable<string> TestGetLocalExtensionsFromFiles()
        {
            var extensionPath = Path.Combine(InstallPath, Constants.ExtensionsInstallDirectoryName);
            return GetLocalExtensionsFromFiles(extensionPath);
        }

        private void OnUpdateCompleted()
        {
            UpdateCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    public interface ITestExtensionManager
    {
        IEnumerable<string> TestGetLocalExtensionsFromFiles();
    }
}