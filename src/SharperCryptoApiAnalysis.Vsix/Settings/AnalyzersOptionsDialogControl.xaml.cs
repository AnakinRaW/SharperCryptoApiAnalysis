using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.Vsix.Settings
{
    public partial class AnalyzersOptionsDialogControl : INotifyPropertyChanged
    {
        private IEnumerable<ISharperCryptoApiAnalysisAnalyzer> _analyzers;
        public IAnalyzerManager Manager { get; }

        public IEnumerable<ISharperCryptoApiAnalysisAnalyzer> Analyzers
        {
            get => _analyzers;
            set
            {
                if (Equals(value, _analyzers)) return;
                _analyzers = value;
                OnPropertyChanged();
            }
        }

        public AnalyzersOptionsDialogControl()
        {
            InitializeComponent();
        }

        public AnalyzersOptionsDialogControl(IAnalyzerManager manager) : this()
        {
            Manager = manager;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
