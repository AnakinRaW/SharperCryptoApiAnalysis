using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;

namespace SdkSampleExtension.NetFramework.CryptoTask
{
    public sealed class TestWizardModel : CryptoCodeGenerationTaskModelBase
    {
        public TestWizardModel()
        {
            FileName = "Test.cs";
        }
    }
}