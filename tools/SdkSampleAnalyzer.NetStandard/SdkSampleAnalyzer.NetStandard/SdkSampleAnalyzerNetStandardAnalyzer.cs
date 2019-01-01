using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SharperCryptoApiAnalysis.Core;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis.Scoring;

namespace SdkSampleAnalyzer.NetStandard
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SdkSampleAnalyzerNetStandardAnalyzer : SharperCryptoApiAnalysisDiagnosticAnalyzer
    {
        //Convention: In order for reports to interact with the VS-Extension all diagnostic id must have the prefix. "SCAA"
        public const string DiagnosticId = DiagnosticPrefix.Prefix + "xxx";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";
        private static NamedLink Link = new NamedLink("Google", new Uri("http://google.de"));


        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        private static IAnalysisReport DefaultReport = new AnalysisReport(DiagnosticId, Description.ToString(), "Long description", "some category", Exploitability.High,
            SecurityGoals.Authenticity | SecurityGoals.Availability, new Uri("http://google.de"), "Learn how to code", new[] { Link });


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override string Name => "Uppercase Test Analyzer";
        public override uint AnalyzerId => 999;

        public override ImmutableArray<IAnalysisReport> SupportedReports => ImmutableArray.Create(DefaultReport);
        public override DiagnosticDescriptor DefaultRule => Rule;

        protected override void InitializeContext(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        //Custom mapping for analysis severity specified in the configuration
        protected override DiagnosticSeverity AnalysisSeverityToDiagnosticSeverity(AnalysisSeverity severity)
        {
            switch (severity)
            {
                case AnalysisSeverity.Default:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Strict:
                    return DiagnosticSeverity.Error;
                case AnalysisSeverity.Medium:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Low:
                    return DiagnosticSeverity.Warning;
                case AnalysisSeverity.Informative:
                    return DiagnosticSeverity.Warning;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.

                var rule = GetRule(DiagnosticId, CurrentSeverity);
                var diagnostic = Diagnostic.Create(rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
