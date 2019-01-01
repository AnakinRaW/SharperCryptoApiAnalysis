using System;
using System.ComponentModel;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels.Extension
{
    public interface IExtensionItemListViewModel : INotifyPropertyChanged
    {
        ExtensionAction Action { get; }
        ISharperCryptoApiExtensionMetadata ExtensionMetadata { get; }
        Version InstalledVersion { get; }
        Version LatestVersion { get; }
        bool Selected { get; set; }

        ExtensionStatus Status { get; }

        bool LatestVersionInstalled { get; }

        bool ReadOnly { get; }

        ExtensionCheckActionDataEntry ToActionDataEntry();
    }
}