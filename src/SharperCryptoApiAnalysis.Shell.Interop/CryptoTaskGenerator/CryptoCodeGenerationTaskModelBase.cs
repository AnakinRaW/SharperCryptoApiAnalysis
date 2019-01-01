using System.ComponentModel;
using System.Runtime.CompilerServices;
using EnvDTE;
using JetBrains.Annotations;

namespace SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator
{
    /// <inheritdoc />
    /// <summary>
    /// Base implementation of an <see cref="T:SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator.ICryptoCodeGenerationTaskModel" /> 
    /// </summary>
    public abstract class CryptoCodeGenerationTaskModelBase : ICryptoCodeGenerationTaskModel
    {
        private string _fileName;

        /// <inheritdoc />
        public virtual string FileName
        {
            get => _fileName;
            set
            {
                if (value == _fileName) return;
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
