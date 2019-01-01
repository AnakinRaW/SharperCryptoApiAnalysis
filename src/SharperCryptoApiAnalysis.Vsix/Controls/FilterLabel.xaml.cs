// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Credits.txt in the project root for license information.
// Original file: NuGet.Client/src/NuGet.Clients/NuGet.PackageManagement.UI/Xamls/FilterLabel.xaml.cs
// Modifications made: Removed unused code and refactorings

using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using SharperCryptoApiAnalysis.Vsix.ViewModels.Extension;

namespace SharperCryptoApiAnalysis.Vsix.Controls
{
    public partial class FilterLabel
    {
        private bool _selected;
        public ExtensionItemFilter Filter { get; set; }

        public event EventHandler<EventArgs> ControlSelected;

        public string Text { get; set; }

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (_selected)
                {
                    TextBlock.SetResourceReference(ForegroundProperty, TreeViewColors.SelectedItemActiveBrushKey);
                    Underline.Visibility = Visibility.Visible;
                    ControlSelected?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    TextBlock.SetResourceReference(ForegroundProperty, VsBrushes.BrandedUITextKey);
                    Underline.Visibility = Visibility.Hidden;
                }
            }
        }

        public FilterLabel()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_selected)
                return;
            Selected = true;
        }

        private void OnTextBlockMouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock.SetResourceReference(ForegroundProperty, TreeViewColors.SelectedItemActiveBrushKey);
        }

        private void OnTextBlockMouseLeave(object sender, MouseEventArgs e)
        {
            if (_selected)
                return;
            TextBlock.SetResourceReference(ForegroundProperty, VsBrushes.BrandedUITextKey);
        }
    }
}
