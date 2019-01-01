using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;

namespace SdkSampleExtension.NetFramework.CryptoTask
{
    //As we do not use the Metadata composition for MEF we need this dummy interface to distinguish 
    public interface ITestWizardPage : ICryptoTaskWizardPage
    {

    }
}