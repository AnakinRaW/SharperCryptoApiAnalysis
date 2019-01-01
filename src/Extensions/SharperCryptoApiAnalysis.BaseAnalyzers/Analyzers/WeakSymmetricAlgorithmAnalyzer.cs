using System;
using System.Collections.Generic;
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
    public class WeakSymmetricAlgorithmAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.WeakSymmetricAlgorithm;

        public override string Name => "Weak Symmetric Algorithm Analyzer";

        public override uint AnalyzerId => AnalysisIds.WeakSymmetricAlgorithmId;

        private static List<string> WeakCipherFactoryNames = new List<string>
        {
            "DESCryptoServiceProvider",
            "TripleDESCryptoServiceProvider",
            "RC2CryptoServiceProvider",
            "DES",
            "System.Security.Cryptography.DES",
            "3DES",
            "TripleDES",
            "Triple DES",
            "System.Security.Cryptography.TripleDES",
            "RC2",
            "System.Security.Cryptography.RC2",
            "http://www.w3.org/2001/04/xmlenc#des-cbc",
            "http://www.w3.org/2001/04/xmlenc#tripledes-cb",
            "http://www.w3.org/2001/04/xmlenc#kw-tripledes"
        };


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.WeakSymmetricAlgorithmReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakSymmetricAlgorithmReport.Summary,
            AnalysisReports.WeakSymmetricAlgorithmReport.Summary, AnalysisReports.WeakSymmetricAlgorithmReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.WeakSymmetricAlgorithmReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakSymmetricAlgorithmReport.Summary,
            AnalysisReports.WeakSymmetricAlgorithmReport.Summary, AnalysisReports.WeakSymmetricAlgorithmReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.WeakSymmetricAlgorithmReport.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakSymmetricAlgorithmReport.Summary,
            AnalysisReports.WeakSymmetricAlgorithmReport.Summary, AnalysisReports.WeakSymmetricAlgorithmReport.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.WeakSymmetricAlgorithmReport.Description);

        protected override void InitializeContext(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ObjectCreationAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(InvocationAction, SyntaxKind.InvocationExpression);
        }

        private void ObjectCreationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ObjectCreationExpressionSyntax objectCreation))
                return;

            if (!objectCreation.IsDerivedFromType(context, typeof(SymmetricAlgorithm)))
                return;

            if  (!objectCreation.IsDerivedFromType(context, typeof(Aes)) && !objectCreation.IsDerivedFromType(context, typeof(Rijndael)))
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    objectCreation.GetLocation(),
                    context.ContainingSymbol.Name));
            }

        }

        private void InvocationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is InvocationExpressionSyntax invocation))
                return;

            if (!invocation.IsDerivedFromType(context, typeof(SymmetricAlgorithm)))
                return;

            if (invocation.IsDerivedFromType(context, typeof(Aes)) ||
                invocation.IsDerivedFromType(context, typeof(Rijndael)))
                return;

            var accessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            var name = accessExpression?.Name;

            if (string.Equals(name?.Identifier.Value.ToString(), nameof(SymmetricAlgorithm.Create),
                StringComparison.Ordinal))
            {
                if (invocation.ArgumentList.Arguments.Count == 0)
                {
                    var typeInfo = context.SemanticModel.GetTypeInfo(invocation);
                    if (typeInfo.Type.Name == nameof(SymmetricAlgorithm))
                        return;
                    var rule = GetRule(DiagnosticId, CurrentSeverity);
                    context.ReportDiagnostic(Diagnostic.Create(rule,
                        invocation.GetLocation(),
                        context.ContainingSymbol.Name));
                    return;
                }

                var firstArgument = invocation.ArgumentList.Arguments[0];

                if (firstArgument.Expression.IsOrContainsCompileTimeConstantValue(context, out string value))
                {
                    if (WeakCipherFactoryNames.Contains(value))
                    {
                        var rule = GetRule(DiagnosticId, CurrentSeverity);
                        context.ReportDiagnostic(Diagnostic.Create(rule,
                            invocation.GetLocation(),
                            context.ContainingSymbol.Name));
                    }
                }
            }
        }
    }
}
