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

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConstantInitializationVectorAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.ConstantIv;

        public override string Name => "Constant IV Analyzer";

        public override uint AnalyzerId => AnalysisIds.ConstantIvId;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.ConstantIvReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ConstantIvReport.Summary,
            AnalysisReports.ConstantIvReport.Summary, AnalysisReports.ConstantIvReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.ConstantIvReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ConstantIvReport.Summary,
            AnalysisReports.ConstantIvReport.Summary, AnalysisReports.ConstantIvReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.ConstantIvReport.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ConstantIvReport.Summary,
            AnalysisReports.ConstantIvReport.Summary, AnalysisReports.ConstantIvReport.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.ConstantIvReport.Description);

        protected override void InitializeContext(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(NodeAction, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(InvocationAction, SyntaxKind.InvocationExpression);
        }

        private void NodeAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is AssignmentExpressionSyntax assignment))
                return;

            if (!(assignment.Left is MemberAccessExpressionSyntax left))
                return;

            if (left.Name.Identifier.Text != nameof(SymmetricAlgorithm.IV))
                return;

            var expression = left.Expression;
            if (expression == null)
                return;

            var memberOwner = context.SemanticModel.GetTypeInfo(expression);
            if (memberOwner.Type.IsDerivedFromType(nameof(SymmetricAlgorithm)))
                AnalyzeInternal(context, assignment.Right, assignment);
        }

        private void InvocationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is InvocationExpressionSyntax invocation))
                return;

            var descendant = invocation.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

            if (descendant == null || !descendant.Expression.IsDerivedFromType(context, typeof(SymmetricAlgorithm)))
                return;

            if (!descendant.Name.Identifier.ValueText.Equals("CreateDecryptor") &&
                !descendant.Name.Identifier.ValueText.Equals("CreateEncryptor"))
                return;

            var arguments = invocation.ArgumentList.Arguments;
            if (arguments.Count != 2)
                return;

            var iv = arguments[1].Expression;

            AnalyzeInternal(context, iv, iv);
        }

        private void AnalyzeInternal(SyntaxNodeAnalysisContext context, ExpressionSyntax analyzeExpression, SyntaxNode reportSyntax)
        {
            if (analyzeExpression.IsOrContainsCompileTimeConstantExpression(context,
                (_, syntax) => IsCompileTimeConstantArrayCreation(syntax)))
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    reportSyntax.GetLocation(),
                    context.ContainingSymbol.Name));
            }
        }

        private static bool IsCompileTimeConstantArrayCreation(ExpressionSyntax syntax)
        {
            if (syntax is ImplicitArrayCreationExpressionSyntax ||
                syntax is ArrayCreationExpressionSyntax || syntax is InitializerExpressionSyntax)
            {
                if (syntax is InitializerExpressionSyntax initializerExpression)
                {
                    return initializerExpression.Kind() == SyntaxKind.ArrayInitializerExpression;
                }

                // The creation of the array does not make is have values. Check if an initializer exists. If not the array might not be constant at all.
                if (!syntax.HasInitializer())
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
