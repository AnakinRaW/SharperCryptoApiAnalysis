using System.ComponentModel.Composition;
using System.Windows;
using SharperCryptoApiAnalysis.Shell.Interop.Wizard;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.SymmetricEncryption.WizardPages
{
    [Export(typeof(ISymmetricEncryptionCryptoTaskWizardPage))]
    public sealed class SymmetricEncryptionCryptoTaskWizardPageUsePassword : WizardPage, ISymmetricEncryptionCryptoTaskWizardPage
    {
        private static readonly FrameworkElement _view = new UsePasswordPage();

        public override string Name => "Encrypt Data with a Key";
        public override string Description => "Choose your secret key origin";
        public override FrameworkElement View => _view;
        public uint SortOrder => 0;
    }
}