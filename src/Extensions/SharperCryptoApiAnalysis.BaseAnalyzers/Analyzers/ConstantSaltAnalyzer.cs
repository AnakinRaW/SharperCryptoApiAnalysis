using System.Collections.Immutable;
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
    public class ConstantSaltAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.ConstantSalt;

        public override string Name => "Constant Salt Analyzer";

        public override uint AnalyzerId => AnalysisIds.ConstantSaltId;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.ConstantSaltReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ConstantSaltReport.Summary,
            AnalysisReports.ConstantSaltReport.Summary, AnalysisReports.ConstantSaltReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.ConstantSaltReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ConstantSaltReport.Summary,
            AnalysisReports.ConstantSaltReport.Summary, AnalysisReports.ConstantSaltReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.ConstantSaltReport.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ConstantSaltReport.Summary,
            AnalysisReports.ConstantSaltReport.Summary, AnalysisReports.ConstantSaltReport.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.ConstantSaltReport.Description);

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

            var saltArgument = argumentList.Arguments[1];
            var e = saltArgument.Expression;

            var type = context.SemanticModel.GetTypeInfo(e).Type;
            if (type?.Kind != SymbolKind.ArrayType)
                return;

            AnalyzeInternal(context, e, saltArgument);
        }

        private void AssignmentAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is AssignmentExpressionSyntax assignment))
                return;

            if (!(assignment.Left is MemberAccessExpressionSyntax left))
                return;

            if (left.Name.Identifier.Text != nameof(Rfc2898DeriveBytes.Salt))
                return;

            var expression = left.Expression;
            if (expression == null)
                return;

            var memberOwner = context.SemanticModel.GetTypeInfo(expression);
            if (memberOwner.Type.IsDerivedFromType(nameof(DeriveBytes)))
                AnalyzeInternal(context, assignment.Right, assignment);
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
