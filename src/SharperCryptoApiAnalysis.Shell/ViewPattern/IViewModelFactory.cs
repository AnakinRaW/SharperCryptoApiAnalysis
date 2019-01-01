﻿using System;
using System.Windows;
using SharperCryptoApiAnalysis.Shell.Interop.ViewManager;
using SharperCryptoApiAnalysis.Shell.ViewModels;

namespace SharperCryptoApiAnalysis.Shell.ViewPattern
{
    /// <summary>
    /// Factory for creating views and view models.
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        /// Creates a view model based on the specified interface type.
        /// </summary>
        /// <typeparam name="TViewModel">The view model interface type.</typeparam>
        /// <returns>The view model.</returns>
        TViewModel CreateViewModel<TViewModel>() where TViewModel : IViewModel;

        /// <summary>
        /// Creates a view based on a view model interface type.
        /// </summary>
        /// <typeparam name="TViewModel">The view model interface type.</typeparam>
        /// <returns>The view.</returns>
        FrameworkElement CreateView<TViewModel>() where TViewModel : IViewModel;

        /// <summary>
        /// Creates a view based on a view model interface type.
        /// </summary>
        /// <param name="viewModel">The view model interface type.</param>
        /// <returns>The view.</returns>
        FrameworkElement CreateView(Type viewModel);
    }
}
