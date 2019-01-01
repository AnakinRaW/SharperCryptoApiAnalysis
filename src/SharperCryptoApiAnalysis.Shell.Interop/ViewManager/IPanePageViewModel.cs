using System;
using System.Threading.Tasks;

namespace SharperCryptoApiAnalysis.Shell.Interop.ViewManager
{
    public interface IPanePageViewModel : IViewModel, IDisposable
    {
        /// <summary>
        /// Gets an exception representing an error state to display.
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// Gets a value indicating whether the page is busy.
        /// </summary>
        /// <remarks>
        /// When <see cref="IsBusy"/> is set to true, an indeterminate progress bar will be
        /// displayed at the top of the GitHub pane but the pane contents will remain visible.
        /// </remarks>
        bool IsBusy { get; }

        /// <summary>
        /// Gets a value indicating whether the page is loading.
        /// </summary>
        /// <remarks>
        /// When <see cref="IsLoading"/> is set to true, a spinner will be displayed instead of the
        /// pane contents.
        /// </remarks>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the title to display in the pane when the page is shown.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Called when the page becomes the current page.
        /// </summary>
        void Activated();

        /// <summary>
        /// Called when the page stops being the current page.
        /// </summary>
        void Deactivated();

        /// <summary>
        /// Refreshes the view model.
        /// </summary>
        Task Refresh();
    }
}
