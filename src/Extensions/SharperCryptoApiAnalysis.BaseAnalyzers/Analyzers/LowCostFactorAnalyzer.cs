using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
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
    public class LowCostFactorAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.LowCostFactor;

        public override string Name => "Low Cost Factor Analyzer";

        public override uint AnalyzerId => AnalysisIds.LowCostFactorId;

        private const int MinimumIterationCount = 10000;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.LowCostFactorReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.LowCostFactorReport.Summary,
            AnalysisReports.LowCostFactorReport.Summary, AnalysisReports.LowCostFactorReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.LowCostFactorReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.LowCostFactorReport.Summary,
            AnalysisReports.LowCostFactorReport.Summary, AnalysisReports.LowCostFactorReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.LowCostFactorReport.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.LowCostFactorReport.Summary,
            AnalysisReports.LowCostFactorReport.Summary, AnalysisReports.LowCostFactorReport.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.LowCostFactorReport.Description);

        protected override void InitializeContext(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ObjectCreationAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(AssignmentAction, SyntaxKind.SimpleAssignmentExpression);
        }

        private void ObjectCreationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ObjectCreationExpressionSyntax objectCreation))
                return;

            var argumentList = objectCreation.ArgumentList;

            var objectCreationTypeInfo = context.SemanticModel.GetTypeInfo(objectCreation);
            if (!objectCreationTypeInfo.Type.IsDerivedFromType(nameof(DeriveBytes)))
                return;


            var blockSyntax = objectCreation.AncestorsAndSelf().OfType<BlockSyntax>().FirstOrDefault();
            var variableDeclaration = objectCreation.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First().Identifier;

            // Prevents checking the Ctor if there is a IterationCount assignment
            if (blockSyntax.AssignsMemberWithName(variableDeclaration, nameof(Rfc2898DeriveBytes.IterationCount)))
                return;


            if (argumentList.Arguments.Count < 3)
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    argumentList.GetLocation(),
                    context.ContainingSymbol.Name));
                return;
            }

            var iterationCount = argumentList.Arguments[2];

            AnalyzeInternal(context, iterationCount.Expression, iterationCount);
        }

        private void AssignmentAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is AssignmentExpressionSyntax assignment))
                return;

            if (!(assignment.Left is MemberAccessExpressionSyntax left))
                return;

            if (left.Name.Identifier.Text != nameof(Rfc2898DeriveBytes.IterationCount))
                return;

            AnalyzeInternal(context, assignment.Right, assignment);
        }

        private void AnalyzeInternal(SyntaxNodeAnalysisContext context, ExpressionSyntax analyzeExpression, SyntaxNode reportSyntax)
        {
            if (analyzeExpression.IsOrContainsCompileTimeConstantValue(context, out int value) ||
                analyzeExpression.HasInvocationWithAtLeastOneReturnExpression(context, (analysisContext, syntax) =>
                    syntax.IsOrContainsCompileTimeConstantValue(context, out value)))
            {
                if (value < MinimumIterationCount)
                {
                    var rule = GetRule(DiagnosticId, CurrentSeverity);
                    context.ReportDiagnostic(Diagnostic.Create(rule,
                        reportSyntax.GetLocation(),
                        context.ContainingSymbol.Name));
                }
            }
        }
    }
}
