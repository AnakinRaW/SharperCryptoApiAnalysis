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
using DiagnosticDescriptor = Microsoft.CodeAnalysis.DiagnosticDescriptor;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
using LanguageNames = Microsoft.CodeAnalysis.LanguageNames;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EqualKeyAndIvAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.EqualKeyAndIv;

        public override string Name => "Equal Key and IV Analyzer";

        public override uint AnalyzerId => AnalysisIds.EqualKeyAndIvId;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.EqualKeyAndIvReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.EqualKeyAndIvReport.Summary,
            AnalysisReports.EqualKeyAndIvReport.Summary, AnalysisReports.EqualKeyAndIvReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.EqualKeyAndIvReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.EqualKeyAndIvReport.Summary,
            AnalysisReports.EqualKeyAndIvReport.Summary, AnalysisReports.EqualKeyAndIvReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.EqualKeyAndIvReport.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.EqualKeyAndIvReport.Summary,
            AnalysisReports.EqualKeyAndIvReport.Summary, AnalysisReports.EqualKeyAndIvReport.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.EqualKeyAndIvReport.Description);

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
            context.RegisterSyntaxNodeAction(ObjectCreationAction, SyntaxKind.VariableDeclarator);
        }

        private void ObjectCreationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is VariableDeclaratorSyntax variableDeclarator))
                return;

            if (variableDeclarator.Initializer == null || !variableDeclarator.Initializer.Value.IsDerivedFromType(context, typeof(SymmetricAlgorithm)))
                return;

            var blockSyntax = variableDeclarator.AncestorsAndSelf().OfType<BlockSyntax>().FirstOrDefault();

            blockSyntax.AssignsMemberWithName(variableDeclarator.Identifier, nameof(SymmetricAlgorithm.IV), out var iv);
            blockSyntax.AssignsMemberWithName(variableDeclarator.Identifier, nameof(SymmetricAlgorithm.Key), out var key);


            if (iv != null && key != null)
            {
                var ivModel = context.SemanticModel.GetSymbolInfo(iv.Right);
                var keyModel = context.SemanticModel.GetSymbolInfo(key.Right);

                if (ivModel.Symbol.Equals(keyModel.Symbol))
                {
                    var lowest = GetLowestSyntaxNode(iv, key);
                    var rule = GetRule(DiagnosticId, CurrentSeverity);
                    context.ReportDiagnostic(Diagnostic.Create(rule,
                        lowest.GetLocation(),
                        context.ContainingSymbol.Name));
                    return;
                }

                byte[] ivArray = null;
                byte[] keyArray = null;
                iv.Right.IsOrContainsCompileTimeConstantExpression(context, (_, syntax) => IsCompileTimeConstantArrayCreation(context, syntax, out ivArray));
                key.Right.IsOrContainsCompileTimeConstantExpression(context, (_, syntax) => IsCompileTimeConstantArrayCreation(context, syntax, out keyArray));

                if (ivArray != null && keyArray != null && ivArray.SequenceEqual(keyArray))
                {
                    var lowest = GetLowestSyntaxNode(iv, key);
                    var rule = GetRule(DiagnosticId, CurrentSeverity);
                    context.ReportDiagnostic(Diagnostic.Create(rule,
                        lowest.GetLocation(),
                        context.ContainingSymbol.Name));
                    return;
                }
            }

            if (iv == null && key != null)
                ReportOnCrossAssignment(context, key, "IV");

            if (key == null && iv != null)
                ReportOnCrossAssignment(context, iv, "Key");
        }

        private static SyntaxNode GetLowestSyntaxNode(SyntaxNode first, SyntaxNode second)
        {
            var firstLine = first.GetLocation().GetLineSpan();
            var secondLine = second.GetLocation().GetLineSpan();

            if (firstLine.StartLinePosition.Line > secondLine.StartLinePosition.Line)
                return first;
            return second;
        }

        private void ReportOnCrossAssignment(SyntaxNodeAnalysisContext context, AssignmentExpressionSyntax assignment, string accesMemeberName)
        {
            if (assignment.Right is MemberAccessExpressionSyntax rightAccessExpression && assignment.Left is MemberAccessExpressionSyntax leftAccessExpression)
            {
                if (rightAccessExpression.Expression.ToString() != leftAccessExpression.Expression.ToString())
                    return;

                if (rightAccessExpression.Name.Identifier.Value.ToString() == accesMemeberName)
                {
                    var rule = GetRule(DiagnosticId, CurrentSeverity);
                    context.ReportDiagnostic(Diagnostic.Create(rule,
                        assignment.Right.GetLocation(),
                        context.ContainingSymbol.Name));
                }
            }
        }

        private static bool IsCompileTimeConstantArrayCreation(SyntaxNodeAnalysisContext context, ExpressionSyntax syntax, out byte[] array)
        {
            array = default(byte[]);
            if (syntax is ImplicitArrayCreationExpressionSyntax ||
                syntax is ArrayCreationExpressionSyntax || syntax is InitializerExpressionSyntax)
            {
                if (syntax is InitializerExpressionSyntax initializerExpression)
                {
                    var flag = initializerExpression.Kind() == SyntaxKind.ArrayInitializerExpression;
                    if (!flag)
                        return false;
                }

                // The creation of the array does not make is have values. Check if an initializer exists. If not the array might not be constant at all.
                if (!syntax.HasInitializer())
                    return false;

                if (syntax is ArrayCreationExpressionSyntax arrayCreation)
                {
                    array = new byte[arrayCreation.Initializer.Expressions.Count];
                    var count = 0;
                    foreach (var expression in arrayCreation.Initializer.Expressions)
                    {
                        expression.IsOrContainsCompileTimeConstantValue(context, out int number);
                        array[count] = (byte) number;
                        count++;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
