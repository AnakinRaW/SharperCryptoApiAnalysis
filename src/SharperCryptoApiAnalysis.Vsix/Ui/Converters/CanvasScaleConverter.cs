// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Credits.txt in the project root for license information.
// Original file: NuGet.Client/src/NuGet.Clients/NuGet.PackageManagement.UI/Xamls/Spinner.xaml.cs
// Modifications made: None

using System.Globalization;
using SharperCryptoApiAnalysis.Shell.Interop.Converters;

namespace SharperCryptoApiAnalysis.Vsix.Ui.Converters
{
    internal class CanvasScaleConverter : ValueConverter<double, double>
    {
        protected override double Convert(double value, object parameter, CultureInfo culture)
        {
            var num = (double)parameter;
            return value / num;
        }
    }
}
