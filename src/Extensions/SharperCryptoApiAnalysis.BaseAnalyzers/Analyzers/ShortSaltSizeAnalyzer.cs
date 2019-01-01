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
    public class ShortSaltSizeAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.ShortSaltSize;

        public override string Name => "Short Salt Size Analyzer";

        public override uint AnalyzerId => AnalysisIds.ShortSaltSizeId;

        private const int MinimumSaltSize = 16;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.ShortSaltSizeReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ShortSaltSizeReport.Summary,
            AnalysisReports.ShortSaltSizeReport.Summary, AnalysisReports.ShortSaltSizeReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.ShortSaltSizeReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ShortSaltSizeReport.Summary,
            AnalysisReports.ShortSaltSizeReport.Summary, AnalysisReports.ShortSaltSizeReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.ShortSaltSizeReport.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.ShortSaltSizeReport.Summary,
            AnalysisReports.ShortSaltSizeReport.Summary, AnalysisReports.ShortSaltSizeReport.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.ShortSaltSizeReport.Description);

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
            if (blockSyntax.AssignsMemberWithName(variableDeclaration, nameof(Rfc2898DeriveBytes.Salt)))
                return;


            if (argumentList.Arguments.Count < 2)
                return;

            var salt = argumentList.Arguments[1];

            AnalyzeInternal(context, salt.Expression, salt);
        }

        private void AssignmentAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is AssignmentExpressionSyntax assignment))
                return;
            if (!(assignment.Left is MemberAccessExpressionSyntax left))
                return;
            if (left.Name.Identifier.Text != nameof(Rfc2898DeriveBytes.Salt))
                return;
            AnalyzeInternal(context, assignment.Right, assignment);
        }

        private void AnalyzeInternal(SyntaxNodeAnalysisContext context, ExpressionSyntax analyzeExpression, SyntaxNode reportSyntax)
        {
            if (analyzeExpression.IsOrContainsCompileTimeConstantExpression(context,
                (_, syntax) => CompileTimeTooShortLiteralOrArray(context, syntax)))
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    reportSyntax.GetLocation(),
                    context.ContainingSymbol.Name));
            }
        }

        private static bool CompileTimeTooShortLiteralOrArray(SyntaxNodeAnalysisContext context, ExpressionSyntax syntax)
        {
            if (syntax is LiteralExpressionSyntax literal && 
                literal.Kind() == SyntaxKind.NumericLiteralExpression)
            {
                var v = context.SemanticModel.GetConstantValue(literal);
                if (!v.HasValue)
                    return false;

                if ((int) v.Value < MinimumSaltSize)
                    return true;
                return false;
            }

            if (syntax is ImplicitArrayCreationExpressionSyntax ||
                syntax is ArrayCreationExpressionSyntax || syntax is InitializerExpressionSyntax)
            {
                if (!syntax.HasInitializer())
                {
                    var rank = syntax.DescendantNodesAndSelf().OfType<ArrayRankSpecifierSyntax>().FirstOrDefault();
                    if (rank == null)
                        return false;
                    var firstRank = rank.Sizes.FirstOrDefault();
                    if (firstRank.IsOrContainsCompileTimeConstantValue<int>(context, out var rankValue))
                        return rankValue < MinimumSaltSize;
                    return false;
                }

                int count;
                if (syntax is ArrayCreationExpressionSyntax arrayCreation)
                    count = arrayCreation.Initializer.Expressions.Count;
                else if (syntax is ImplicitArrayCreationExpressionSyntax implicitArrayCreation)
                    count = implicitArrayCreation.Initializer.Expressions.Count;
                else 
                    count = ((InitializerExpressionSyntax) syntax).Expressions.Count;
                return count < MinimumSaltSize;
            }
            return false;
        }
    }
}
