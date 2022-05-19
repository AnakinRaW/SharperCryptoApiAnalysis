using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommonServiceLocator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SharperCryptoApiAnalysis.Interop.Settings;

namespace SharperCryptoApiAnalysis.Interop.CodeAnalysis
{
    /// <inheritdoc cref="ISharperCryptoApiAnalysisAnalyzer"/>
    /// <summary>
    /// A code analysis provider in Sharper Crypto-API Analysis
    /// </summary>
    /// <seealso cref="T:Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer" />
    /// <seealso cref="T:SharperCryptoApiAnalysis.Interop.CodeAnalysis.ISharperCryptoApiAnalysisAnalyzer" />
    public abstract class SharperCryptoApiAnalysisDiagnosticAnalyzer : DiagnosticAnalyzer, ISharperCryptoApiAnalysisAnalyzer
    {
        /// <inheritdoc />
        /// <summary>
        /// The name.
        /// </summary>
        public abstract string Name { get; }

        /// <inheritdoc />
        /// <summary>
        /// The analyzer identifier.
        /// </summary>
        public abstract uint AnalyzerId { get; }

        /// <inheritdoc />
        /// <summary>
        /// Indicating whether the analyzer is muted.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is muted; otherwise, <see langword="false" />.
        /// </value>
        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                if (value == _isMuted) return;
                _isMuted = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Supported reports.
        /// </summary>
        public abstract ImmutableArray<IAnalysisReport> SupportedReports { get; }

        /// <summary>
        /// The default diagnostic.
        /// </summary>
        public abstract DiagnosticDescriptor DefaultRule { get; }

        /// <summary>
        /// The responsible manager.
        /// </summary>
        protected IAnalyzerManager Manager { get; set; }


        /// <summary>
        /// The settings provider
        /// </summary>
        private ISettingsProvider _settingsProvider;

        private bool _isMuted;

        /// <summary>
        /// Gets the current severity of the analyzer.
        /// </summary>
        protected AnalysisSeverity CurrentSeverity
        {
            get
            {
                if (ServiceLocator.IsLocationProviderSet)
                {
                    if (_settingsProvider == null)
                        _settingsProvider = ServiceLocator.Current.GetInstance<ISettingsProvider>();
                    
                    if (_settingsProvider.Settings == null)
                        return AnalysisSeverity.Default;
                    return _settingsProvider.Settings.Severity;
                }

                return AnalysisSeverity.Default;
            }
        }

        protected SharperCryptoApiAnalysisDiagnosticAnalyzer()
        {
            if (!ServiceLocator.IsLocationProviderSet)
                return;
            var manager = ServiceLocator.Current.GetInstance<IAnalyzerManager>();
            if (manager == null)
                return;
            Manager = manager;
            Manager.RegisterAnalyzer(this);
        }

        public sealed override void Initialize(AnalysisContext context)
        {
            if (IsMuted)
                return;
            InitializeContext(context);
        }

        protected abstract void InitializeContext(AnalysisContext context);

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="report">The report.</param>
        protected void SendReport(IAnalysisReport report)
        {
            Manager?.Report(report);
        }

        /// <summary>
        /// Maps an <see cref="AnalysisSeverity"/> to a <see cref="DiagnosticSeverity"/>.
        /// </summary>
        /// <param name="severity">The <see cref="AnalysisSeverity"/> </param>
        /// <returns>The mapped <see cref="DiagnosticSeverity"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual DiagnosticSeverity AnalysisSeverityToDiagnosticSeverity(AnalysisSeverity severity)
        {
            switch (severity)
            {
                case AnalysisSeverity.Default:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Strict:
                    return DiagnosticSeverity.Error;
                case AnalysisSeverity.Medium:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Low:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Informative:
                    return DiagnosticSeverity.Info;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the rule from a given <see cref="AnalysisSeverity"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="analysisSeverity">The analysis severity.</param>
        /// <returns>The rule</returns>
        protected DiagnosticDescriptor GetRule(string id, AnalysisSeverity analysisSeverity)
        {
            var rule = SupportedDiagnostics.FirstOrDefault(x =>
                x.Id.Equals(id) && x.DefaultSeverity == AnalysisSeverityToDiagnosticSeverity(analysisSeverity));
            if (rule == null)
                return DefaultRule;
            return rule;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}