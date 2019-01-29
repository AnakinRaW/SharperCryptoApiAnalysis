using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SharperCryptoApiAnalysis.Shell.Interop.Wizard;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <inheritdoc cref="ICryptoTaskWizardProvider" />
    /// <summary>
    /// Base implementation of an <see cref="ICryptoTaskWizardProvider"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="T:SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator.ICryptoTaskWizardProvider" />
    /// <seealso cref="T:System.ComponentModel.Composition.IPartImportsSatisfiedNotification" />
    public abstract class CryptoTaskWizardProvider<T> : ICryptoTaskWizardProvider, IPartImportsSatisfiedNotification where T : ICryptoTaskWizardPage
    {
        [ImportMany] private IEnumerable<T> _unsortedPaged;

        /// <inheritdoc />
        /// <summary>
        /// Gets a value indicating whether this instance is empty wizard.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this instance is empty wizard; otherwise, <see langword="false" />.
        /// </value>
        public virtual bool IsEmptyWizard => false;

        /// <inheritdoc />
        /// <summary>
        /// List of sorted wizard pages
        /// </summary>
        public IReadOnlyCollection<IWizardPage> SortedPages { get; private set; }

        public void OnImportsSatisfied()
        {
            var sorted = _unsortedPaged.ToList().OrderBy(x => x.SortOrder).OfType<IWizardPage>().ToList();

            IWizardPage current = null;
            for (var i = 0; i < sorted.Count; i++)
            {
                var page = sorted[i];
                if (page == null)
                    return;

                page.PreviousPage = current;
                current = page;
                page.NextPage = i + 1 < sorted.Count ? sorted[i + 1] : null;
            }
            SortedPages = sorted;
        }
    }
}
