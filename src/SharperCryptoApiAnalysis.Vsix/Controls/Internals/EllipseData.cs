// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Original file: NuGet.Client/src/NuGet.Clients/NuGet.PackageManagement.UI/Xamls/Spinner.xaml.cs
// Modifications made: None

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace SharperCryptoApiAnalysis.Vsix.Controls.Internals
{
    internal class EllipseData : ObservableCollection<EllipseDetails>
    {
        private static readonly double[] LeftCoordinates = {
            20.1696, 2.86816, 5.03758e-006, 12.1203, 36.5459, 64.6723, 87.6176, 98.165, 92.9838, 47.2783
        };

        private static readonly double[] TopCoordinates = {
            9.76358, 29.9581, 57.9341, 83.3163, 98.138, 96.8411, 81.2783, 54.414, 26.9938, 0.5
        };

        private static readonly int[] Opacities = {
            0xE6, 0xCD, 0xB3, 0x9A, 0x80, 0x67, 0x4D, 0x34, 0x1A, 0xFF
        };

        private readonly SolidColorBrush _indicatorFill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#007ACC"));

        public EllipseData()
        {
            var baseColor = _indicatorFill.Color;

            Enumerable.Range(0, LeftCoordinates.Length)
                .Select(i => new EllipseDetails
                {
                    Width = 21.835,
                    Height = 21.862,
                    Left = LeftCoordinates[i],
                    Top = TopCoordinates[i],
                    Fill = new SolidColorBrush(Color.FromArgb((byte)Opacities[i], baseColor.R, baseColor.G, baseColor.B))
                })
                .ToList()
                .ForEach(e => Add(e));
        }
    }
}
