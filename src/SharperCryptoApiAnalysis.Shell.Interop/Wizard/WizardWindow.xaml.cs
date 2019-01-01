namespace SharperCryptoApiAnalysis.Shell.Interop.Wizard
{
    public partial class WizardWindow
    {
        public WizardWindow()
        {
            InitializeComponent();
        }

        public WizardWindow(WizardViewModel viewModel) : this()
        {
            DataContextChanged += OnDataContextChanged;
            DataContext = viewModel;         
        }

        private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is WizardViewModel oldViewModel)
                oldViewModel.Finished -= OnFinished;
            if (e.NewValue != null && e.NewValue is WizardViewModel newViewModel)
                newViewModel.Finished += OnFinished;

        }

        private void OnFinished(object sender, object e)
        {
            DataContext = null;
            DialogResult = true;
            Close();
        }
    }
}
