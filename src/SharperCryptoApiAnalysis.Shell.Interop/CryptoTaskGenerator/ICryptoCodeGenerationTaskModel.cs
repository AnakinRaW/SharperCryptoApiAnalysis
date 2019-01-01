using System.ComponentModel;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <inheritdoc />
    /// <summary>
    /// The data model of a crypto task
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public interface ICryptoCodeGenerationTaskModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The file name of the generate crypto task
        /// </summary>
        string FileName { get; set; }
    }
}