using System;
using System.Collections.Generic;
using System.Linq;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.Settings;

namespace SharperCryptoApiAnalysis.Vsix.Settings
{
    public partial class GeneralOptionsDialogControl
    {
        public ISharperCryptoApiAnalysisSettings Settings { get; }

        public IEnumerable<AnalysisSeverity> Severities => Enum.GetValues(typeof(AnalysisSeverity)).Cast<AnalysisSeverity>();

        public GeneralOptionsDialogControl()
        {
            InitializeComponent();
        }

        public GeneralOptionsDialogControl(ISharperCryptoApiAnalysisSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            InitializeComponent();
        }
    }
}
