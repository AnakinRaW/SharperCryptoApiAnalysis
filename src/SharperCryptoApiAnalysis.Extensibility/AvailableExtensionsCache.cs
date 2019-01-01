using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.Configuration;
using SharperCryptoApiAnalysis.Interop.Extensibility;
using SharperCryptoApiAnalysis.Interop.Services;

namespace SharperCryptoApiAnalysis.Extensibility
{
    internal class AvailableExtensionsCache : IAvailableExtensionsCache
    {
        private readonly IConfigurationManager _configurationManager;
        private AvailableExtensionsList _extensions;

        public IReadOnlyCollection<ISharperCryptoApiExtensionMetadata> Extensions
        {
            get
            {
                if (_extensions == null)
                {
                    var extensionPath = Path.Combine(_configurationManager.ExtensionManager.InstallPath, Constants.ExtensionsInstallDirectoryName);
                    if (!Directory.Exists(extensionPath))
                        return new List<ISharperCryptoApiExtensionMetadata>();

                    if (!_configurationManager.HasConfiguration)
                        return new List<ISharperCryptoApiExtensionMetadata>();

                    var list = AvailableExtensionsMetadataFile.OpenCreate(extensionPath, _configurationManager.ExtensionManager.LocalExtensions);

                    if (_configurationManager.Configuration.SyncMode == ConfigSyncMode.Hard)
                        list.Clear();
                    else
                    {
                        foreach (var metadata in list.Where(x => !x.External || x.External && !string.IsNullOrEmpty(x.Source)).ToList())
                            list.Remove(metadata);
                    }


                    foreach (var extension in _configurationManager.Configuration.Extensions)
                        list.Add(extension);

                    _extensions = list;
                }

                return _extensions.ToList();
            }
        }

        public AvailableExtensionsCache(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _extensions = new AvailableExtensionsList();
        }

        public void Invalidate()
        {
            _extensions = null;
        }
    }

    public interface IAvailableExtensionsCache
    {
        IReadOnlyCollection<ISharperCryptoApiExtensionMetadata> Extensions { get; }

        void Invalidate();
    }
}