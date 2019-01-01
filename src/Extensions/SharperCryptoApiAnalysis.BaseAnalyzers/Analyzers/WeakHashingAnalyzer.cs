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
    public class WeakHashingAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        public const string DiagnosticId = AnalysisIds.WeakHashing;

        public override string Name => "Weak Hashing Analyzer";

        public override uint AnalyzerId => AnalysisIds.WeakHashingId;

        private static List<string> WeakHashingFactoryNames = new List<string>
        {
            "SHA",
            "SHA1",
            "System.Security.Cryptography.SHA1",
            "System.Security.Cryptography.SHA1Cng",
            "MD5",
            "System.Security.Cryptography.MD5",
            "System.Security.Cryptography.MD5Cng",
            "RIPEMD160",
            "RIPEMD-160",
            "System.Security.Cryptography.RIPEMD160",
            "System.Security.Cryptography.RIPEMD160Managed",
            "HMACMD5",
            "System.Security.Cryptography.HMACMD5",
            "HMACRIPEMD160",
            "System.Security.Cryptography.HMACRIPEMD160",
            "System.Security.Cryptography.MACTripleDES",
            "http://www.w3.org/2000/09/xmldsig#sha1",
            "http://www.w3.org/2001/04/xmlenc#ripemd160",
            "http://www.w3.org/2001/04/xmldsig-more#hmac-md5",
            "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160"
        };


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WarningRule, InformationRule, ErrorRule, HiddenRule);

        public override ImmutableArray<IAnalysisReport> SupportedReports =>
            ImmutableArray.Create(AnalysisReports.WeakHashingReport);

        public override DiagnosticDescriptor DefaultRule => WarningRule;

        private static readonly DiagnosticDescriptor WarningRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakHashingReport.Summary,
            AnalysisReports.WeakHashingReport.Summary, AnalysisReports.WeakHashingReport.Category,
            DiagnosticSeverity.Warning, true, AnalysisReports.WeakHashingReport.Description);

        private static readonly DiagnosticDescriptor ErrorRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakHashingReport.Summary,
            AnalysisReports.WeakHashingReport.Summary, AnalysisReports.WeakHashingReport.Category,
            DiagnosticSeverity.Error, true, AnalysisReports.WeakHashingReport.Description);

        private static readonly DiagnosticDescriptor InformationRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakHashingReport.Summary,
            AnalysisReports.WeakHashingReport.Summary, AnalysisReports.WeakHashingReport.Category,
            DiagnosticSeverity.Info, true, AnalysisReports.WeakHashingReport.Description);

        private static readonly DiagnosticDescriptor HiddenRule = new DiagnosticDescriptor(DiagnosticId,
            AnalysisReports.WeakHashingReport.Summary,
            AnalysisReports.WeakHashingReport.Summary, AnalysisReports.WeakHashingReport.Category,
            DiagnosticSeverity.Hidden, true, AnalysisReports.WeakHashingReport.Description);

        protected override void InitializeContext(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ObjectCreationAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(InvocationAction, SyntaxKind.InvocationExpression);
        }

        protected override DiagnosticSeverity AnalysisSeverityToDiagnosticSeverity(AnalysisSeverity severity)
        {
            switch (severity)
            {
                case AnalysisSeverity.Default:
                    return DiagnosticSeverity.Info;
                case AnalysisSeverity.Strict:
                    return DiagnosticSeverity.Error;
                case AnalysisSeverity.Medium:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Low:
                    return DiagnosticSeverity.Hidden;
                case AnalysisSeverity.Informative:
                    return DiagnosticSeverity.Info;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        private void ObjectCreationAction(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ObjectCreationExpressionSyntax objectCreation))
                return;

            if (!objectCreation.IsDerivedFromType(context, typeof(HashAlgorithm)))
            {
                if (objectCreation.IsDerivedFromType(context, typeof(DeriveBytes)))
                {
                    if (objectCreation.ArgumentList.Arguments.Count != 4)
                        return;

                    var hashModeExpression = objectCreation.ArgumentList.Arguments[3].Expression;

                    if (hashModeExpression is MemberAccessExpressionSyntax accessExpression)
                    {
                        if (accessExpression.Name.Identifier.Value.Equals("MD5"))
                        {
                            var rule = GetRule(DiagnosticId, CurrentSeverity);
                            context.ReportDiagnostic(Diagnostic.Create(rule,
                                objectCreation.GetLocation(),
                                context.ContainingSymbol.Name));
                            return;
                        }
                    }
                }
                return;
            }


            if (!IsSecureHashingFunction(context, objectCreation))
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

            if (!invocation.IsDerivedFromType(context, typeof(HashAlgorithm)))
                return;

            if (IsSecureHashingFunction(context, invocation))
                return;

            var accessExpression = invocation.Expression as MemberAccessExpressionSyntax;
            var name = accessExpression?.Name;

            if (!string.Equals(name?.Identifier.Value.ToString(), nameof(HashAlgorithm.Create),
                StringComparison.Ordinal))
                return;

            if (invocation.ArgumentList.Arguments.Count == 0)
            {
                var rule = GetRule(DiagnosticId, CurrentSeverity);
                context.ReportDiagnostic(Diagnostic.Create(rule,
                    invocation.GetLocation(),
                    context.ContainingSymbol.Name));
                return;
            }

            var firstArgument = invocation.ArgumentList.Arguments[0];
            if (firstArgument.Expression.IsOrContainsCompileTimeConstantValue(context, out string value))
            {
                if (WeakHashingFactoryNames.Contains(value))
                {
                    var rule = GetRule(DiagnosticId, CurrentSeverity);
                    context.ReportDiagnostic(Diagnostic.Create(rule,
                        invocation.GetLocation(),
                        context.ContainingSymbol.Name));
                }     
            }
        }

        private static bool IsSecureHashingFunction(SyntaxNodeAnalysisContext context, ExpressionSyntax objectCreation)
        {
            if (objectCreation.IsDerivedFromType(context, typeof(SHA256)) ||
                objectCreation.IsDerivedFromType(context, typeof(SHA384)) ||
                objectCreation.IsDerivedFromType(context, typeof(SHA512)))
                return true;

            if (objectCreation.IsDerivedFromType(context, typeof(HMACSHA256)) ||
                objectCreation.IsDerivedFromType(context, typeof(HMACSHA384)) ||
                objectCreation.IsDerivedFromType(context, typeof(HMACSHA512)) ||
                //NIST is fine with HMAC SHA-1 so we should be too
                objectCreation.IsDerivedFromType(context, typeof(HMACSHA1)))
                return true;

            return false;
        }
    }
}
