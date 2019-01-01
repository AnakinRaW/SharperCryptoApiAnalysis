using SharperCryptoApiAnalysis.Shell.Interop.CryptoTaskGenerator;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.SymmetricEncryption
{
    public sealed class SymmetricEncryptionCryptoTaskModel : CryptoCodeGenerationTaskModelBase
    {
        private SecurityLevel _selectedSecurityLevel;
        private bool _useMac;
        private bool _usePassword;

        public SecurityLevel SelectedSecurityLevel
        {
            get => _selectedSecurityLevel;
            set
            {
                if (value == _selectedSecurityLevel) return;
                _selectedSecurityLevel = value;
                OnPropertyChanged();
            }
        }

        public bool UseMac
        {
            get => _useMac;
            set
            {
                if (value == _useMac) return;
                _useMac = value;
                OnPropertyChanged();
            }
        }

        public bool UsePassword
        {
            get => _usePassword;
            set
            {
                if (value == _usePassword) return;
                _usePassword = value;
                OnPropertyChanged();
            }
        }

        public SymmetricEncryptionCryptoTaskModel()
        {
            FileName = "SymmetricCryptoProvider.cs";
            SelectedSecurityLevel = SecurityLevel.Default;
        }
    }
}
