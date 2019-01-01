using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.SymmetricEncryption.WizardPages;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.SymmetricEncryption
{
    [Export(typeof(SymmetricEncryptionCryptoTaskWizardProvider))]
    public class SymmetricEncryptionCryptoTaskWizardProvider : CryptoTaskWizardProvider<ISymmetricEncryptionCryptoTaskWizardPage>
    {

    }
}
