using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;

namespace SharperCryptoApiAnalysis.Shell.Interop.Wizard
{
    /// <inheritdoc />
    /// <summary>
    /// View model of a wizard
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public abstract class WizardViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the wizard is finished.
        /// </summary>
        internal event EventHandler<object> Finished;

        private IWizardPage _currentPage;

        /// <summary>
        /// The title of the wizard.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// The watermark.
        /// </summary>
        public virtual string WatermarkText { get; }

        /// <summary>
        /// The next page command.
        /// </summary>
        public ICommand NextPageCommand => new DelegateCommand(NavigateNextPage);

        /// <summary>
        /// The previous page command.
        /// </summary>
        public ICommand PreviousPageCommand => new DelegateCommand(NavigatePreviousPage);

        /// <summary>
        /// The finish command.
        /// </summary>
        public ICommand FinishCommand => new DelegateCommand(ExecuteFinish);

        /// <summary>
        /// Gets or sets a value indicating whether the wizard is completed.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if completed; otherwise, <see langword="false"/>.
        /// </value>
        public bool Completed { get; protected set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public IWizardPage CurrentPage
        {
            get => _currentPage;
            protected set
            {
                if (Equals(value, _currentPage)) return;
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The result of the Wizard.
        /// </summary>
        public object Result { get; protected set; }

        protected virtual void NavigateNextPage()
        {
            if (CurrentPage.NextPage != null)
                CurrentPage = CurrentPage.NextPage;
        }

        protected virtual void NavigatePreviousPage()
        {
            if (CurrentPage.PreviousPage != null)
                CurrentPage = CurrentPage.PreviousPage;
        }

        protected abstract void Finish();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnFinished(object e)
        {
            Completed = true;
            Finished?.Invoke(this, e);
        }

        private void ExecuteFinish()
        {
            Finish();
            OnFinished(Result);
        }
    }
}
