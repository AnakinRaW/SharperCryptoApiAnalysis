using System;
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
    public class ConstantKeyAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.ConstantKey;

        public override string Name => "Constant Key Analyzer";

        public override uint AnalyzerId => AnalysisIds.ConstantKeyId;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.ConstantKeyReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ConstantKeyReport.Summary,
            AnalysisReports.ConstantKeyReport.Summary, AnalysisReports.ConstantKeyReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.ConstantKeyReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ConstantKeyReport.Summary,
            AnalysisReports.ConstantKeyReport.Summary, AnalysisReports.ConstantKeyReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.ConstantKeyReport.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ConstantKeyReport.Summary,
            AnalysisReports.ConstantKeyReport.Summary, AnalysisReports.ConstantKeyReport.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.ConstantKeyReport.Description);

        protected override DiagnosticSeverity AnalysisSeverityToDiagnosticSeverity(AnalysisSeverity severity)
        {
            switch (severity)
            {
                case AnalysisSeverity.Default:
                case AnalysisSeverity.Strict:
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

            if (left.Name.Identifier.Text != nameof(SymmetricAlgorithm.Key))
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

            var key = arguments[0].Expression;

            AnalyzeInternal(context, key, key);
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
