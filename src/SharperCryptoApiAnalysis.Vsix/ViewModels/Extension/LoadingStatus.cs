// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Credits.txt in the project root for license information.
// Original file: NuGet.Client/src/NuGet.Clients/NuGet.PackageManagement.VisualStudio/PackageFeeds/LoadingStatus.cs
// Modifications made: Removed comments and NoMoreItems value 

namespace SharperCryptoApiAnalysis.Vsix.ViewModels.Extension
{
    public enum LoadingStatus
    {
        Unknown,
        Cancelled,
        ErrorOccurred,
        Loading,
        NoItemsFound,
        Ready
    }
}