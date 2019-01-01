using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SharperCryptoApiAnalysis.BaseAnalyzers.Reports;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Extensions;
using DiagnosticDescriptor = Microsoft.CodeAnalysis.DiagnosticDescriptor;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
using LanguageNames = Microsoft.CodeAnalysis.LanguageNames;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WeakRandomAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.WeakRandom;

        public override string Name => "Weak Random Analyzer";

        public override uint AnalyzerId => AnalysisIds.WeakRandomId;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, HiddenRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.WeakRandomReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakRandomReport.Summary,
            AnalysisReports.WeakRandomReport.Summary, AnalysisReports.WeakRandomReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.WeakRandomReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakRandomReport.Summary,
            AnalysisReports.WeakRandomReport.Summary, AnalysisReports.WeakRandomReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.WeakRandomReport.Description);

        private static readonly DiagnosticDescriptor HiddenRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakRandomReport.Summary,
            AnalysisReports.WeakRandomReport.Summary, AnalysisReports.WeakRandomReport.Category,
            DiagnosticSeverity.Hidden, true, AnalysisReports.WeakRandomReport.Description);

        protected override void InitializeContext(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ObjectCreationAction, SyntaxKind.ObjectCreationExpression);
        }

        protected override DiagnosticSeverity AnalysisSeverityToDiagnosticSeverity(AnalysisSeverity severity)
        {
            switch (severity)
            {
                case AnalysisSeverity.Default:
                    return DiagnosticSeverity.Hidden;
                case AnalysisSeverity.Strict:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Medium:
                    return DiagnosticSeverity.Hidden;
                case AnalysisSeverity.Low:
                    return DiagnosticSeverity.Hidden;
                case AnalysisSeverity.Informative:
                    return DiagnosticSeverity.Info;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        private void ObjectCreationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ObjectCreationExpressionSyntax objectCreation))
                return;

            var objectCreationTypeInfo = context.SemanticModel.GetTypeInfo(objectCreation);
            if (!objectCreationTypeInfo.Type.IsDerivedFromType(nameof(Random)))
                return;

            var rule = GetRule(DiagnosticId, CurrentSeverity);
            context.ReportDiagnostic(Diagnostic.Create(rule,
                objectCreation.GetLocation(),
                context.ContainingSymbol.Name));

        }
    }
}
