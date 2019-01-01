using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.BaseAnalyzers.Reports;
using SharperCryptoApiAnalysis.BaseAnalyzers.Tests.Verifiers;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    [TestClass]
    public class WeakRandomAnalyzerTests : CodeFixVerifier
    {
        public static IAnalysisReport Report = AnalysisReports.WeakRandomReport;

        [TestMethod]
        public void SystemRandomCtor()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static int Random()
        {
            var random = new Random();
            return random.Next();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Hidden,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 26)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new Analyzers.WeakRandomAnalyzer();
        }
    }
}