using System;
using System.Collections.Immutable;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using System.Linq;
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
    public class WeakOperationModeAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.WeakOperationMode;

        public override string Name => "Weak Cipher Operation Mode Analyzer";

        public override uint AnalyzerId => AnalysisIds.WeakOperationModeId;

        private static readonly string InformationSummary = "Insecure encryption operation mode";

        private static readonly string InformationDescription =
            "Operation mode ECB must be avoided at any case. All other modes the .NET Api provides, including CBC, are vulnerable.";


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.WeakOperationModeReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakOperationModeReport.Summary,
            AnalysisReports.WeakOperationModeReport.Summary, AnalysisReports.WeakOperationModeReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.WeakOperationModeReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            InformationSummary, InformationSummary, AnalysisReports.WeakOperationModeReport.Category,
            DiagnosticSeverity.Info, true, InformationDescription);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId, AnalysisReports.WeakOperationModeReport.Summary,
            AnalysisReports.WeakOperationModeReport.Summary, AnalysisReports.WeakOperationModeReport.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.WeakOperationModeReport.Description);

        protected override void InitializeContext(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AssignmentExpressionAction, SyntaxKind.SimpleAssignmentExpression);
        }

        protected override DiagnosticSeverity AnalysisSeverityToDiagnosticSeverity(AnalysisSeverity severity)
        {
            switch (severity)
            {
                case AnalysisSeverity.Default:
                    return DiagnosticSeverity.Error;
                case AnalysisSeverity.Strict:
                    return DiagnosticSeverity.Error;
                case AnalysisSeverity.Medium:
                    return DiagnosticSeverity.Error;
                case AnalysisSeverity.Low:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Informative:
                    return DiagnosticSeverity.Info;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        private void AssignmentExpressionAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is AssignmentExpressionSyntax assignment))
                return;

            var variableDeclaration = assignment.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
            var blockSyntax = variableDeclaration?.AncestorsAndSelf().OfType<BlockSyntax>().FirstOrDefault();

            if (variableDeclaration != null && blockSyntax.AssignsMemberWithName(variableDeclaration.Identifier, nameof(SymmetricAlgorithm.Mode)))
                return;

            if (assignment.Left is MemberAccessExpressionSyntax memberAccess)
            {
                if (!memberAccess.Name.Identifier.ValueText.Equals(nameof(SymmetricAlgorithm.Mode)))
                    return;
            }

            if (assignment.Left is IdentifierNameSyntax identifierName)
            {
                if (!identifierName.Identifier.ValueText.Equals(nameof(SymmetricAlgorithm.Mode)))
                    return;
            }

            CipherMode value;
            if (assignment.Right.IsOrContainsCompileTimeConstantValue(context, out value) && value == CipherMode.ECB)
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    assignment.Right.GetLocation(),
                    context.ContainingSymbol.Name));
            }
            else
            {
                if (CurrentSeverity == AnalysisSeverity.Informative)
                {
                    context.ReportDiagnostic(Diagnostic.Create(InformationRule,
                        assignment.Right.GetLocation(),
                        context.ContainingSymbol.Name));
                }
            }
        }
    }
}
