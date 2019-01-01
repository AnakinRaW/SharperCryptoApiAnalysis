using System;
using System.Composition;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.SymmetricEncryption
{
    [Export(typeof(ICryptoCodeGenerationTask))]
    internal class SymmetricEncryptionCryptoTask : ICryptoCodeGenerationTask
    {
        public string Name => "Encrypt and decrypt data with a Key";

        public string Description => "This tasks generates code for the simple usage of encryption and decryption data based on a secret key.\r\n" +
                                     "Depending on your needs the secret key can be derived from a password.";

        public ICryptoCodeGenerationTaskModel Model { get; private set; }

        public ICryptoCodeGenerationTaskHandler TaskHandler { get; }

        public ICryptoTaskWizardProvider WizardProvider { get; }

        [ImportingConstructor]
        public SymmetricEncryptionCryptoTask(SymmetricEncryptionCryptoTaskWizardProvider wizardProvider, SymmetricEncryptionCryptoTaskHandler taskHandler)
        {
            WizardProvider = wizardProvider;
            TaskHandler = taskHandler;
        }

        public ICryptoCodeGenerationTaskModel CreateAndSetModel()
        {
            Model = new SymmetricEncryptionCryptoTaskModel();
            return Model;
        }

        public CompatibleProjectTypes CompatibleProjectTypes =>
            CompatibleProjectTypes.Framework | CompatibleProjectTypes.Core | CompatibleProjectTypes.StandardV2;
    }
}
