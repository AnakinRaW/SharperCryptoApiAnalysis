// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Original file: NuGet.Client/src/NuGet.Clients/NuGet.PackageManagement.UI/Xamls/PackageManagerTopPanel.xaml.cs
// Modifications made: Removed unused code, added SelectedFilter dependency property, adjusted filter behaviour

using System;
using System.Windows;
using SharperCryptoApiAnalysis.Vsix.ViewModels.Extension;

namespace SharperCryptoApiAnalysis.Vsix.Controls
{
    public partial class ExtensionManagerNavigationPanel
    {
        public event EventHandler<FilterChangedEventArgs> FilterChanged;

        private FilterLabel _selectedLabel;

        public static readonly DependencyProperty SelectedFilterProperty = DependencyProperty.Register(
            "SelectedFilter", typeof(ExtensionItemFilter), typeof(ExtensionManagerNavigationPanel), new PropertyMetadata(default(ExtensionItemFilter), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ExtensionManagerNavigationPanel panel))
                return;

            var value = (ExtensionItemFilter) e.NewValue;
            panel.SelectFilterLabel(value);
        }


        public ExtensionItemFilter SelectedFilter
        {
            get => (ExtensionItemFilter) GetValue(SelectedFilterProperty);
            set => SetValue(SelectedFilterProperty, value);
        }


        public ExtensionManagerNavigationPanel()
        {
            InitializeComponent();
            SelectFilterLabel(ExtensionItemFilter.Available);
        }

        public void FilterLabel_ControlSelected(object sender, EventArgs e)
        {
            var filterLabel = (FilterLabel)sender;
            if (filterLabel == _selectedLabel)
                return;
            if (_selectedLabel != null)
                _selectedLabel.Selected = false;
            var selectedFilter = _selectedLabel;
            _selectedLabel = filterLabel;

            SelectedFilter = _selectedLabel.Filter;

            FilterChanged?.Invoke(this, new FilterChangedEventArgs(selectedFilter?.Filter));
        }

        private void SelectFilterLabel(ExtensionItemFilter selectedFilter)
        {
            if (_selectedLabel != null)
                _selectedLabel.Selected = false;
            switch (selectedFilter)
            {
                case ExtensionItemFilter.Available:
                    _selectedLabel = AvailableLabel;
                    break;
                case ExtensionItemFilter.Installed:
                    _selectedLabel = InstalledLabel;
                    break;
                case ExtensionItemFilter.Update:
                    _selectedLabel = UpdateLabel;
                    break;
            }
            if (_selectedLabel == null)
                _selectedLabel = InstalledLabel;
            _selectedLabel.Selected = true;
        }
    }
}
