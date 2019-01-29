using SharperCryptoApiAnalysis.Shell.Interop.Wizard;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <inheritdoc />
    /// <summary>
    /// Page used for crypto task wizard
    /// </summary>
    /// <seealso cref="T:SharperCryptoApiAnalysis.Shell.Interop.Wizard.IWizardPage" />
    public interface ICryptoTaskWizardPage : IWizardPage
    {
        /// <summary>
        /// The sort order of the page.
        /// </summary>
        uint SortOrder { get; }
    }
}
