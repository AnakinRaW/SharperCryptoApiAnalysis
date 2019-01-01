// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Credits.txt in the project root for license information.
// Original file: NuGet.Client/src/NuGet.Clients/NuGet.PackageManagement.UI/LoadingStatusIndicator.cs
// Modifications made: Removed error message behavior

using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SharperCryptoApiAnalysis.Vsix.ViewModels.Extension
{
    internal class LoadingStatusIndicator : INotifyPropertyChanged
    {
        private LoadingStatus _status;
        private string _loadingMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadingStatus Status
        {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                if (value == _loadingMessage) return;
                _loadingMessage = value;
                OnPropertyChanged();
            }
        }

        public void Reset(string loadingMessage)
        {
            Status = LoadingStatus.Unknown;
            LoadingMessage = loadingMessage;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}