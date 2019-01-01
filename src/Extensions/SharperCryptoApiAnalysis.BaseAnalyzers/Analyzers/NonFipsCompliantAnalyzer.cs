using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Internal.VisualStudio.PlatformUI;
using SharperCryptoApiAnalysis.BaseAnalyzers.Reports;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Extensions;
using DiagnosticDescriptor = Microsoft.CodeAnalysis.DiagnosticDescriptor;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
using LanguageNames = Microsoft.CodeAnalysis.LanguageNames;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonFipsCompliantAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.NonFipsCompliant;

        public override string Name => "Weak Hashing Analyzer";

        public override uint AnalyzerId => AnalysisIds.NonFipsCompliantId;

        private static List<string> NonFipsNames = new List<string>
        {
            "AesManaged", "System.Security.Cryptography.AesManaged",
            "SHA256", "SHA-256", "System.Security.Cryptography.SHA256",
            "SHA384", "SHA-384", "System.Security.Cryptography.SHA384",
            "SHA512", "SHA-512", "System.Security.Cryptography.SHA512",
        };


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.NonFipsCompliantReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.NonFipsCompliantReport.Summary,
            AnalysisReports.NonFipsCompliantReport.Summary, AnalysisReports.NonFipsCompliantReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.NonFipsCompliantReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.NonFipsCompliantReport.Summary,
            AnalysisReports.NonFipsCompliantReport.Summary, AnalysisReports.NonFipsCompliantReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.NonFipsCompliantReport.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.NonFipsCompliantReport.Summary,
            AnalysisReports.NonFipsCompliantReport.Summary, AnalysisReports.NonFipsCompliantReport.Category,
            DiagnosticSeverity.Hidden, true, AnalysisReports.NonFipsCompliantReport.Description);

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

        protected override void InitializeContext(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ObjectCreationAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(InvocationAction, SyntaxKind.InvocationExpression);
        }

        private void ObjectCreationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ObjectCreationExpressionSyntax objectCreation))
                return;

            if (objectCreation.IsDerivedFromType(context, typeof(SymmetricAlgorithm)))
            {
                AnalyzeSymmetricAlgorithmObjectCreation(context, objectCreation);
                return;
            }

            if (objectCreation.IsDerivedFromType(context, typeof(HashAlgorithm)))
                AnalyzeHashAlgorithmObjectCreation(context, objectCreation);
        }

        private void InvocationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is InvocationExpressionSyntax invocation))
                return;

            var accessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            var name = accessExpression?.Name;
            if (!string.Equals(name?.Identifier.Value.ToString(), nameof(HashAlgorithm.Create),
                StringComparison.Ordinal))
                return;

            if (invocation.IsDerivedFromType(context, typeof(SymmetricAlgorithm)))
            {
                AnalyzeSymmetricAlgorithmFactory(context, invocation);
                return;
            }

            if (invocation.IsDerivedFromType(context, typeof(HashAlgorithm)))
                AnalyzeHashingFactory(context, invocation);

        }

        private void AnalyzeSymmetricAlgorithmFactory(SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocation)
        {
            if (invocation.IsDerivedFromType(context, typeof(DES)) ||
                invocation.IsDerivedFromType(context, typeof(TripleDES)) ||
                invocation.IsDerivedFromType(context, typeof(RC2)))
                return;

            if (invocation.IsDerivedFromType(context, typeof(Rijndael)) ||
                invocation.IsDerivedFromType(context, typeof(AesManaged)))
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    invocation.GetLocation(),
                    context.ContainingSymbol.Name));
                return;
            }

            if (invocation.ArgumentList.Arguments.Count == 0)
            {
                if (invocation.IsOfType(context, typeof(SymmetricAlgorithm)))
                {
                    var rule = GetRule(DiagnosticId, CurrentSeverity);
                    context.ReportDiagnostic(Diagnostic.Create(rule,
                        invocation.GetLocation(),
                        context.ContainingSymbol.Name));
                }
                return;
            }

            var firstArgument = invocation.ArgumentList.Arguments[0];

            if (firstArgument.Expression.IsOrContainsCompileTimeConstantValue(context, out string value))
            {
                if (NonFipsNames.Contains(value))
                {
                    var rule = GetRule(DiagnosticId, CurrentSeverity);
                    context.ReportDiagnostic(Diagnostic.Create(rule,
                        invocation.GetLocation(),
                        context.ContainingSymbol.Name));
                }
            }
        }

        private void AnalyzeHashingFactory(SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocation)
        {
            if (invocation.IsDerivedFromType(context, typeof(MD5)))
                return;

            if (invocation.IsDerivedFromType(context, typeof(RIPEMD160)) ||
                invocation.IsDerivedFromType(context, typeof(HMACRIPEMD160)))
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    invocation.GetLocation(),
                    context.ContainingSymbol.Name));
                return;
            }

            if (invocation.ArgumentList.Arguments.Count == 0)
            {
                if (invocation.IsOfType(context, typeof(HMAC)) ||
                    invocation.IsOfType(context, typeof(HashAlgorithm)) ||
                    invocation.IsOfType(context, typeof(KeyedHashAlgorithm)) ||
                    invocation.IsOfType(context, typeof(SHA1)))
                    return;
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    invocation.GetLocation(),
                    context.ContainingSymbol.Name));
                return;
            }
        }

        private void AnalyzeSymmetricAlgorithmObjectCreation(SyntaxNodeAnalysisContext context, ObjectCreationExpressionSyntax objectCreation)
        {
            if (objectCreation.IsDerivedFromType(context, typeof(DES)) ||
                objectCreation.IsDerivedFromType(context, typeof(TripleDES)) ||
                objectCreation.IsDerivedFromType(context, typeof(RC2)))
                return;

            if (objectCreation.IsDerivedFromType(context, typeof(Rijndael)) ||
                objectCreation.IsDerivedFromType(context, typeof(AesManaged)))
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    objectCreation.GetLocation(),
                    context.ContainingSymbol.Name));
                return;
            }

            if (objectCreation.ArgumentList == null || !objectCreation.ArgumentList.Arguments.Any())
                return;

            var argument = objectCreation.ArgumentList?.Arguments[1];
            if (argument.Expression.IsOrContainsCompileTimeConstantValue(context, out bool value) && value)
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    objectCreation.GetLocation(),
                    context.ContainingSymbol.Name));
            }
        }

        private void AnalyzeHashAlgorithmObjectCreation(SyntaxNodeAnalysisContext context, ObjectCreationExpressionSyntax objectCreation)
        {

            if (objectCreation.IsDerivedFromType(context, typeof(MD5)) ||
                objectCreation.IsDerivedFromType(context, typeof(HMACSHA256)) ||
                objectCreation.IsDerivedFromType(context, typeof(HMACSHA384)) ||
                objectCreation.IsDerivedFromType(context, typeof(HMACSHA512)) ||
                objectCreation.IsDerivedFromType(context, typeof(MACTripleDES)))
                return;

            if (objectCreation.IsDerivedFromType(context, typeof(RIPEMD160)) ||
                objectCreation.IsDerivedFromType(context, typeof(SHA1Managed)) ||
                objectCreation.IsDerivedFromType(context, typeof(SHA256Managed)) ||
                objectCreation.IsDerivedFromType(context, typeof(SHA384Managed)) ||
                objectCreation.IsDerivedFromType(context, typeof(SHA512Managed)) ||
                objectCreation.IsDerivedFromType(context, typeof(HMACRIPEMD160)))
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    objectCreation.GetLocation(),
                    context.ContainingSymbol.Name));
                return;
            }

            if (objectCreation.IsDerivedFromType(context, typeof(HMACSHA1)) && objectCreation.ArgumentList.Arguments.Count == 2)
            {
                var argument = objectCreation.ArgumentList.Arguments[1];

                if (argument.Expression.IsOrContainsCompileTimeConstantValue(context, out bool value) && value)
                {
                    var rule = GetRule(DiagnosticId, CurrentSeverity);
                    context.ReportDiagnostic(Diagnostic.Create(rule,
                        objectCreation.GetLocation(),
                        context.ContainingSymbol.Name));
                }
            }
        }
    }
}
