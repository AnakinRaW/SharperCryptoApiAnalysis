using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;

namespace SharperCryptoApiAnalysis.VisualStudio.Integration
{
    /// <summary>
    /// Represents some properties of a Visual Studio installation
    /// </summary>
    public class VisualStudio
    {
        /// <summary>
        /// The Name of the Version
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The install Path
        /// </summary>
        public string InstallPath { get; }

        /// <summary>
        /// The install id
        /// </summary>
        public string InstallId { get; }

        /// <summary>
        /// The installed version
        /// </summary>
        public Version Version { get; }

        private static readonly Dictionary<string, VisualStudio> InstanceCache = new Dictionary<string, VisualStudio>();

        /// <summary>
        /// The user specific local path of the visual studio instance
        /// </summary>
        public string UserLocalPath
        {
            get
            {
                var localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(localPath,
                    $"Microsoft\\VisualStudio\\{Version.Major}.{Version.MajorRevision}_{InstallId}\\");
            }
        }

        public VisualStudio(string name, string installPath, string installId, string version)
        {
            if (string.IsNullOrEmpty(installPath))
                throw new ArgumentNullException(nameof(installPath));
            if (string.IsNullOrEmpty(installId))
                throw new ArgumentNullException(nameof(installId));
            if (!Directory.Exists(installPath))
                throw new ArgumentException("VS not found");
            InstallPath = installPath;
            InstallId = installId;
            Version = Version.Parse(version);
            if (Version.Major < 14)
                throw new VersionMismatchException("Version must be at least 14.0");
            Name = name;
        }

        /// <summary>
        /// Clears the MEF catalog
        /// </summary>
        public void ClearComponentModelCache()
        {
            var componentModelCacheFolder = Path.Combine(UserLocalPath, @"ComponentModelCache\");
            if (Directory.Exists(componentModelCacheFolder))
                Directory.Delete(componentModelCacheFolder, true);
        }

        /// <summary>
        /// Clears the extensions cache
        /// </summary>
        public void ClearExtensionCache()
        {
            var extensionsDir = Path.Combine(UserLocalPath, @"Extensions\");
            var cacheFiles = Directory.GetFiles(extensionsDir, "extensions.*.cache", SearchOption.TopDirectoryOnly);

            foreach (var cache in cacheFiles)
            {
                if (File.Exists(cache))
                    File.Delete(cache);
            }
        }

        /// <summary>
        /// Gets an installation instance
        /// </summary>
        /// <param name="configurationSuffix"></param>
        /// <returns></returns>
        public static VisualStudio GetInstance(string configurationSuffix = "")
        {
            if (InstanceCache.ContainsKey(configurationSuffix))
                return InstanceCache[configurationSuffix];

            var isProcess = ProcessHelper.IsRunningInsideVisualStudio();

            var instance = isProcess
                ? VisualStudioSetupProvider.GetRunningVisualStudio()
                : VisualStudioSetupProvider.GetInstalledVisualStudio(configurationSuffix);

            InstanceCache.Add(configurationSuffix, instance);
            return instance;
        }

        /// <summary>
        /// Restarts the IDE
        /// </summary>
        public void Restart()
        {
            var shell = Services.SharperCryptoAnalysisServiceProvider.GetService<SVsShell>() as IVsShell4;
            shell?.Restart((uint) __VSRESTARTTYPE.RESTART_Normal);
        }
    }
}