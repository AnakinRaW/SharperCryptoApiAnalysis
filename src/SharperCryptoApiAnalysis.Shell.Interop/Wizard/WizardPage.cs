using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;

namespace SharperCryptoApiAnalysis.Shell.Interop.Wizard
{
    /// <summary>
    /// Basic <see cref="IWizardPage"/> implementation
    /// </summary>
    /// <seealso cref="SharperCryptoApiAnalysis.Shell.Interop.Wizard.IWizardPage" />
    public abstract class WizardPage : IWizardPage
    {
        private IWizardPage _nextPage;
        private IWizardPage _previousPage;
        private bool _canFinish;
        private object _dataModel;

        /// <summary>
        /// The name of the page.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The description of the page.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// The view of the page
        /// </summary>
        public abstract FrameworkElement View { get; }

        /// <summary>
        /// The data model.
        /// </summary>
        public virtual object DataModel
        {
            get => _dataModel;
            set
            {
                if (Equals(value, _dataModel)) return;
                _dataModel = value;
                OnPropertyChanged();
                OnDataModelChanged();
            }
        }

        /// <summary>
        /// The next page.
        /// </summary>
        public IWizardPage NextPage
        {
            get => _nextPage;
            set
            {
                if (Equals(value, _nextPage)) return;
                _nextPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNextPage));
            }
        }

        /// <summary>
        /// The previous page.
        /// </summary>
        public IWizardPage PreviousPage
        {
            get => _previousPage;
            set
            {
                if (Equals(value, _previousPage)) return;
                _previousPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasPreviousPage));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this page can finish the wizard.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this instance can finish; otherwise, <see langword="false" />.
        /// </value>
        public virtual bool CanFinish
        {
            get => _canFinish;
            protected set
            {
                if (value == _canFinish) return;
                _canFinish = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicating whether this page has next page.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this instance has next page; otherwise, <see langword="false" />.
        /// </value>
        public bool HasNextPage => NextPage != null;

        /// <summary>
        /// Indicating whether this page has previous page.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this instance has previous page; otherwise, <see langword="false" />.
        /// </value>
        public bool HasPreviousPage => PreviousPage != null;

        protected WizardPage(WizardPage previousPage, WizardPage nextPage, object dataModel)
        {
            _previousPage = previousPage;
            _nextPage = nextPage;
            _dataModel = dataModel;
        }

        protected WizardPage(WizardPage nextPage, object dataModel) : this(null, nextPage, dataModel)
        {
        }

        protected WizardPage(object dataModel, WizardPage previousPage) : this(previousPage, null, dataModel)
        {
        }

        protected WizardPage(object dataModel) : this(null, null, dataModel)
        {
        }

        protected WizardPage()
        {

        }

        protected virtual void OnDataModelChanged()
        {
            if (View != null)
                View.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
