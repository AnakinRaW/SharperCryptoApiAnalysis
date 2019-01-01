using System.Collections.Generic;
using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;
using SharperCryptoApiAnalysis.Shell.Interop.Wizard;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.KeyGeneration
{
    [Export(typeof(ICryptoCodeGenerationTask))]
    internal class SecureKeyGenerationTask : ICryptoCodeGenerationTask
    {
        public string Name => "Generate secure random Keys";
        public string Description => "Generating random keys is essential for secure cryptography. " +
                                     "This task will implement a key generator that returns secure keys based on a user specified bit length.";
        public ICryptoCodeGenerationTaskModel Model { get; private set; }
        public ICryptoCodeGenerationTaskHandler TaskHandler { get; }
        public ICryptoTaskWizardProvider WizardProvider => new EmptyWizardProvider();

        public ICryptoCodeGenerationTaskModel CreateAndSetModel()
        {
            Model = new EmptyModel();
            return Model;
        }

        [ImportingConstructor]
        internal SecureKeyGenerationTask(SecureCodeGenerationTaskHandler taskHandler)
        {
            TaskHandler = taskHandler;
        }

        public CompatibleProjectTypes CompatibleProjectTypes =>
            CompatibleProjectTypes.Framework | CompatibleProjectTypes.Core | CompatibleProjectTypes.StandardV2;

        private sealed class EmptyModel : CryptoCodeGenerationTaskModelBase
        {
            public EmptyModel()
            {
                FileName = "SecureKeyGenerator.cs";
            }
        }

        private class EmptyWizardProvider : ICryptoTaskWizardProvider
        {
            public bool IsEmptyWizard => true;
            public IReadOnlyCollection<IWizardPage> SortedPages => new List<IWizardPage>();
        }
    }
}
