using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SharperCryptoApiAnalysis.Shell.Interop.Wizard;
using SharperCryptoApiAnalysis.Vsix.Controls;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <inheritdoc />
    /// <summary>
    /// Crypto task wizard view model
    /// </summary>
    /// <seealso cref="T:SharperCryptoApiAnalysis.Shell.Interop.Wizard.WizardViewModel" />
    public sealed class CryptoTaskWizardViewModel : WizardViewModel
    {
        private readonly ICryptoCodeGenerationTaskProvider _provider;
        private ICryptoCodeGenerationTask _selectedTaskItem;

        private readonly SelectCryptoTaskWizardPage _selectCryptoTaskPage;

        /// <summary>
        /// The watermark.
        /// </summary>
        public override string WatermarkText => "Crypto Task Wizard";

        /// <summary>
        /// Gets the available tasks.
        /// </summary>
        /// <value>
        /// The available tasks.
        /// </value>
        public IEnumerable<ICryptoCodeGenerationTask> AvailableTasks { get; }

        /// <summary>
        /// Gets or sets the selected task item.
        /// </summary>
        /// <value>
        /// The selected task item.
        /// </value>
        public ICryptoCodeGenerationTask SelectedTaskItem
        {
            get => _selectedTaskItem;
            set
            {
                if (Equals(value, _selectedTaskItem)) return;
                _selectedTaskItem = value;
                OnPropertyChanged();
                OnSelectedTaskItemChanged(value);
            }
        }

        /// <summary>
        /// The selected crypto task.
        /// </summary>
        public ICryptoCodeGenerationTask SelectedTask { get; private set; }

        public CryptoTaskWizardViewModel(ICryptoCodeGenerationTaskProvider provider)
        {
            _provider = provider;
            AvailableTasks = _provider.Tasks;
            _selectCryptoTaskPage = new SelectCryptoTaskWizardPage(this);
            CurrentPage = _selectCryptoTaskPage;
        }

        /// <summary>
        /// The title of the wizard.
        /// </summary>
        public override string Title => "Crypto Task Wizard";

        protected override void Finish()
        {
            Result = SelectedTask;
            Completed = true;
        }

        private void OnSelectedTaskItemChanged(ICryptoCodeGenerationTask task)
        {
            SelectedTask = task;
            if (task == null)
                return;
            _selectCryptoTaskPage.OnTaskSet(task);
        }

        private sealed class SelectCryptoTaskWizardPage : WizardPage
        {
            public override string Name => "Crypto Task Selection";
            public override string Description => "Select the cryptography task you want to implement.";
            public override FrameworkElement View { get; }

            public SelectCryptoTaskWizardPage(CryptoTaskWizardViewModel viewModel) : base(viewModel)
            {
                View = new SelectCryptoTaskPage { DataContext = this };
            }

            internal void OnTaskSet(ICryptoCodeGenerationTask task)
            {
                CanFinish = false;

                if (task == null)
                {
                    NextPage = null;
                    return;
                }

                var pages = task.WizardProvider.SortedPages;
                var model = task.CreateAndSetModel();

                if (task.WizardProvider.IsEmptyWizard)
                {
                    CanFinish = true;
                    return;
                }

                if (pages == null || pages.Count == 0)
                {
                    NextPage = null;
                    return;
                }

                foreach (var page in pages)
                    page.DataModel = model;

                var firstPage = pages.FirstOrDefault();
                if (firstPage == null)
                {
                    NextPage = null;
                    return;
                }



                firstPage.PreviousPage = this;
                NextPage = firstPage;


            }
        }
    }
}
