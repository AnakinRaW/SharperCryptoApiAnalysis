using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;

namespace SdkSampleExtension.NetFramework.CryptoTask
{
    [Export(typeof(ICryptoCodeGenerationTask))]
    internal class TestTask : ICryptoCodeGenerationTask
    {
        public string Name => "TestTask";
        public string Description => "Test Task Description";

        public ICryptoCodeGenerationTaskModel Model { get; private set; }
        public ICryptoCodeGenerationTaskHandler TaskHandler { get; }

        public ICryptoTaskWizardProvider WizardProvider { get; }

        public ICryptoCodeGenerationTaskModel CreateAndSetModel()
        {
            var model = new TestWizardModel();
            Model = model;
            return model;
        }

        public CompatibleProjectTypes CompatibleProjectTypes => CompatibleProjectTypes.Framework;

        [ImportingConstructor]
        public TestTask(TestTaskWizardProvider wizardProvider, TestCodeGenerationTaskHandler handler)
        {
            WizardProvider = wizardProvider;
            TaskHandler = handler;
        }
    }
}
