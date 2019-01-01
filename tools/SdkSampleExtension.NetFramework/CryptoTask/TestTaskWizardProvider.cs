using System.ComponentModel.Composition;
using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;

namespace SdkSampleExtension.NetFramework.CryptoTask
{

    //As we do not use the Metadata composition for MEF we need this dummy class to distinguish 
    [Export(typeof(TestTaskWizardProvider))]
    internal class TestTaskWizardProvider : CryptoTaskWizardProvider<ITestWizardPage>
    {

    }
}