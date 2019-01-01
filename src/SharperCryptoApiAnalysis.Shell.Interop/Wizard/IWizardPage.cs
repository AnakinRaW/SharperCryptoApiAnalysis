using System.ComponentModel;
using System.Windows;

namespace SharperCryptoApiAnalysis.Shell.Interop.Wizard
{
    /// <summary>
    /// A page of a wizard
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IWizardPage : INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the page.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the page.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The view of the page
        /// </summary>
        FrameworkElement View { get; }

        /// <summary>
        /// The data model.
        /// </summary>
        object DataModel { get; set; }

        /// <summary>
        /// The next page.
        /// </summary>
        IWizardPage NextPage { get; set; }

        /// <summary>
        /// The previous page.
        /// </summary>
        IWizardPage PreviousPage { get; set; }

        /// <summary>
        /// Indicating whether this page has next page.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance has next page; otherwise, <see langword="false"/>.
        /// </value>
        bool HasNextPage { get; }

        /// <summary>
        /// Indicating whether this page has previous page.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance has previous page; otherwise, <see langword="false"/>.
        /// </value>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Gets a value indicating whether this page can finish the wizard.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance can finish; otherwise, <see langword="false"/>.
        /// </value>
        bool CanFinish { get; }
    }
}
