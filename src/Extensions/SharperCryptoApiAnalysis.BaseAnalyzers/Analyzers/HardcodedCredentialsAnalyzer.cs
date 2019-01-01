using System;
using System.Collections.Generic;
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
    public class HardcodedCredentialsAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.HardcodedCredentials;

        public override string Name => "Hardcoded Credentials Analyzer";

        public override uint AnalyzerId => AnalysisIds.HardcodedCredentialsId;

        public List<string> SuspiciousNames => new List<string>
        {
            "pass", "password", "pw", "username", "user", "passwd", "admin", "administrator"
        };


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, HiddenRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.HardcodedCredentialsReprot);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.HardcodedCredentialsReprot.Summary,
            AnalysisReports.HardcodedCredentialsReprot.Summary, AnalysisReports.HardcodedCredentialsReprot.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.HardcodedCredentialsReprot.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.HardcodedCredentialsReprot.Summary,
            AnalysisReports.HardcodedCredentialsReprot.Summary, AnalysisReports.HardcodedCredentialsReprot.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.HardcodedCredentialsReprot.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.HardcodedCredentialsReprot.Summary,
            AnalysisReports.HardcodedCredentialsReprot.Summary, AnalysisReports.HardcodedCredentialsReprot.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.HardcodedCredentialsReprot.Description);

        private static readonly DiagnosticDescriptor HiddenRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.HardcodedCredentialsReprot.Summary,
            AnalysisReports.HardcodedCredentialsReprot.Summary, AnalysisReports.HardcodedCredentialsReprot.Category,
            DiagnosticSeverity.Hidden, true, AnalysisReports.HardcodedCredentialsReprot.Description);

        protected override void InitializeContext(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(VariableDeclaratorAction, SyntaxKind.VariableDeclarator);
            context.RegisterSyntaxNodeAction(PropertyDeclarationAction, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(ObjectCreationAction, SyntaxKind.ObjectCreationExpression);
        }

        protected override DiagnosticSeverity AnalysisSeverityToDiagnosticSeverity(AnalysisSeverity severity)
        {
            switch (severity)
            {
                case AnalysisSeverity.Default:
                    return DiagnosticSeverity.Info;
                case AnalysisSeverity.Strict:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Medium:
                    return DiagnosticSeverity.Info;
                case AnalysisSeverity.Low:
                    return DiagnosticSeverity.Hidden;
                case AnalysisSeverity.Informative:
                    return DiagnosticSeverity.Info;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        private void VariableDeclaratorAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is VariableDeclaratorSyntax variableDeclarator))
                return;

            if (variableDeclarator.Initializer == null)
                return;

            if (!variableDeclarator.Initializer.Value.IsOrContainsCompileTimeConstantValue<string>(context, out _))
                return;

            var variableName = variableDeclarator.Identifier.ValueText;
            AnalyzeInternal(context, variableName, variableDeclarator.Initializer);

        }

        private void ObjectCreationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ObjectCreationExpressionSyntax objectCreation))
                return;

            var argumentList = objectCreation.ArgumentList;

            var objectCreationTypeInfo = context.SemanticModel.GetTypeInfo(objectCreation);
            if (!objectCreationTypeInfo.Type.IsDerivedFromType(nameof(DeriveBytes)))
                return;

            if (argumentList.Arguments.Count < 1)
                return;

            var password = argumentList.Arguments[0];

            AnalyzeSyntaxInternal(context, password.Expression, password);
        }

        private void PropertyDeclarationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is PropertyDeclarationSyntax propertyDeclaration))
                return;

            if (propertyDeclaration.Initializer == null)
                return;

            if (!propertyDeclaration.Initializer.Value.IsOrContainsCompileTimeConstantValue<string>(context, out _))
                return;

            var variableName = propertyDeclaration.Identifier.ValueText;

            AnalyzeInternal(context, variableName, propertyDeclaration.Initializer);
        }

        private void AnalyzeSyntaxInternal(SyntaxNodeAnalysisContext context, ExpressionSyntax syntax, SyntaxNode reportSyntax)
        {
            if (syntax.IsOrContainsCompileTimeConstantExpression(context,
                (_, expressionSyntax) => IsCompileTimeConstantPassword(context, expressionSyntax)))
            {
                context.ReportDiagnostic(Diagnostic.Create(ErrorRule,
                    reportSyntax.GetLocation(),
                    context.ContainingSymbol.Name));
            }
        }

        private void AnalyzeInternal(SyntaxNodeAnalysisContext context, string variableName, SyntaxNode reportSyntax)
        {
            if (SuspiciousNames.Any(x => variableName.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0))
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    reportSyntax.GetLocation(),
                    context.ContainingSymbol.Name));
            }
        }

        private static bool IsCompileTimeConstantPassword(SyntaxNodeAnalysisContext context, ExpressionSyntax syntax)
        {
            if (syntax is LiteralExpressionSyntax literal &&
                literal.Kind() == SyntaxKind.StringLiteralExpression)
            {
                var v = context.Compilation.GetSemanticModel(syntax.SyntaxTree).GetConstantValue(literal);
                if (!v.HasValue)
                    return false;
                return true;
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
                    return firstRank.IsOrContainsCompileTimeConstantValue<int>(context, out _);
                }

                return true;
            }

            return false;
        }
    }
}
