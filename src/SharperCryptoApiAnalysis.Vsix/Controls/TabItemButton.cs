// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Credits.txt in the project root for license information.
// Original file: NuGet.Client/src/NuGet.Clients/NuGet.PackageManagement.UI/Controls/TabItemButton.cs
// Modifications made: Removed automation code

using System.Windows.Controls;

namespace SharperCryptoApiAnalysis.Vsix.Controls
{
    internal class TabItemButton : Button
    {
        public void Select()
        {
            OnClick();
        }
    }
}
