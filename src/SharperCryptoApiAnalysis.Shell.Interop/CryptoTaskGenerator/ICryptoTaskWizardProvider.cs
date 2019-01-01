using System.Collections.Generic;
using SharperCryptoApiAnalysis.Shell.Interop.Wizard;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <summary>
    /// Provider for all wizard pages of a crypto task
    /// </summary>
    public interface ICryptoTaskWizardProvider
    {
        /// <summary>
        /// Gets a value indicating whether this instance is empty wizard.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance is empty wizard; otherwise, <see langword="false"/>.
        /// </value>
        bool IsEmptyWizard { get; }

        /// <summary>
        /// List of sorted wizard pages
        /// </summary>
        IReadOnlyCollection<IWizardPage> SortedPages { get; }
    }
}
