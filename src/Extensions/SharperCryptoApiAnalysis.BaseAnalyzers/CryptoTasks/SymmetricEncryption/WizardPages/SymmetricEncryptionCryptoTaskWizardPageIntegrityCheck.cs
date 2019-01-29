using System.ComponentModel.Composition;
using System.Windows;
using SharperCryptoApiAnalysis.Shell.Interop.Wizard;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.SymmetricEncryption.WizardPages
{
    [Export(typeof(ISymmetricEncryptionCryptoTaskWizardPage))]
    public sealed class SymmetricEncryptionCryptoTaskWizardPageIntegrityCheck : WizardPage, ISymmetricEncryptionCryptoTaskWizardPage
    {
        private static readonly FrameworkElement _view = new UseMacPage();


        public override string Name => "Encrypt Data with a Key";
        public override string Description => "Choose additional security features";
        public override FrameworkElement View => _view;
        public uint SortOrder => 2;

        public SymmetricEncryptionCryptoTaskWizardPageIntegrityCheck()
        {
            CanFinish = true;
        }
    }
}