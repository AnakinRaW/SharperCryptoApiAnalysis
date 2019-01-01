using System;
using System.Threading.Tasks;

namespace SharperCryptoApiAnalysis.Shell.Interop.ViewManager
{
    /// <summary>
    /// Base class for view models that appear as a page in a the GitHub pane.
    /// </summary>
    public abstract class PanePageViewModelBase : ViewModelBase, IPanePageViewModel
    {
        private Exception _error;
        private bool _isLoading;
        private bool _isBusy;

        /// <summary>
        /// Gets an exception representing an error state to display.
        /// </summary>
        public Exception Error
        {
            get => _error;
            protected set
            {
                if (Equals(value, _error)) return;
                _error = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the page is busy.
        /// </summary>
        /// <remarks>
        /// When <see cref="IsBusy" /> is set to true, an indeterminate progress bar will be
        /// displayed at the top of the GitHub pane but the pane contents will remain visible.
        /// </remarks>
        public bool IsBusy
        {
            get => _isBusy;
            protected set
            {
                if (value == _isBusy) return;
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the page is loading.
        /// </summary>
        /// <remarks>
        /// When <see cref="IsLoading" /> is set to true, a spinner will be displayed instead of the
        /// pane contents.
        /// </remarks>
        public bool IsLoading
        {
            get => _isLoading;
            protected set
            {
                if (value == _isLoading) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the title to display in the pane when the page is shown.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Called when the page becomes the current page.
        /// </summary>
        public virtual void Activated()
        {
        }

        /// <summary>
        /// Called when the page stops being the current page.
        /// </summary>
        public virtual void Deactivated()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Refreshes the view model.
        /// </summary>
        /// <returns></returns>
        public virtual Task Refresh() => Task.CompletedTask;

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}