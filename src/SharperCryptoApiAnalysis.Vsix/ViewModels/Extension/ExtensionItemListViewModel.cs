using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharperCryptoApiAnalysis.Interop.Extensibility;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels.Extension
{
    internal class ExtensionItemListViewModel : IExtensionItemListViewModel
    {
        private bool _selected;
        private ExtensionStatus _status;

        public bool Selected
        {
            get => _selected;
            set
            {
                if (value == _selected)
                    return;
                _selected = value;
                OnPropertyChanged();
            }
        }

        public bool ReadOnly { get; }

        public ISharperCryptoApiExtensionMetadata ExtensionMetadata { get; }

        public Version InstalledVersion { get; }

        public Version LatestVersion { get; }

        public ExtensionAction Action { get; }

        public ExtensionStatus Status
        {
            get => _status;
            private set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public bool LatestVersionInstalled =>
            Status == ExtensionStatus.Installed &&
            (InstalledVersion == LatestVersion || LatestVersion == null);

        public ExtensionItemListViewModel(ISharperCryptoApiExtensionMetadata metadata, Version installedVersion, Version latestVersion, bool readOnly = false)
        {
            ExtensionMetadata = metadata;
            InstalledVersion = installedVersion;
            LatestVersion = latestVersion;
            Action = ExtensionAction.DoNothing;
            ReadOnly = readOnly;
            SetStatus();
        }

        public ExtensionItemListViewModel(ExtensionCheckActionDataEntry dataEntry, bool readOnly = false)
        {
            ExtensionMetadata = dataEntry.ExtensionMetadata;
            InstalledVersion = dataEntry.InstalledVersion;
            LatestVersion = dataEntry.ExtensionMetadata.Version;
            Action = dataEntry.Action;
            ReadOnly = readOnly;
            SetStatus();
        }

        public ExtensionCheckActionDataEntry ToActionDataEntry()
        {
            return new ExtensionCheckActionDataEntry(ExtensionMetadata, Action, InstalledVersion);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetStatus()
        {
            if (InstalledVersion == null)
                Status = ExtensionStatus.NotInstalled;
            else
            {
                if (LatestVersion != null && LatestVersion != InstalledVersion)
                    Status = ExtensionStatus.UpdateAvailable;
                else
                    Status = ExtensionStatus.Installed;
            }
        }
    }

    public enum ExtensionStatus
    {
        NotInstalled,
        Installed,
        UpdateAvailable
    }
}