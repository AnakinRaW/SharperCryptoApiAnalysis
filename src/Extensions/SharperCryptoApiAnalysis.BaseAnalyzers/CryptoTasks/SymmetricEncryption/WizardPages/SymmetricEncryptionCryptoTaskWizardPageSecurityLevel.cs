using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using SharperCryptoApiAnalysis.Shell.Interop.Wizard;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.CryptoTasks.SymmetricEncryption.WizardPages
{
    [Export(typeof(ISymmetricEncryptionCryptoTaskWizardPage))]
    public sealed class SymmetricEncryptionCryptoTaskWizardPageSecurityLevel : WizardPage, ISymmetricEncryptionCryptoTaskWizardPage
    {
        private static FrameworkElement _view = new SecurityLevelPage();

        public override string Name => "Encrypt Data with a Key";
        public override string Description => "Choose your encryption mode";
        public override FrameworkElement View => _view;
        public uint SortOrder => 1;

        public IEnumerable<SecurityLevel> SecurityLevels => Enum.GetValues(typeof(SecurityLevel)).Cast<SecurityLevel>();
    }
}
