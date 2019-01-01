using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers;
using SharperCryptoApiAnalysis.BaseAnalyzers.CodeFixes;
using SharperCryptoApiAnalysis.BaseAnalyzers.Reports;
using SharperCryptoApiAnalysis.BaseAnalyzers.Tests.Verifiers;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    [TestClass]
    public class ConstantKeyAnalyzerTests : CodeFixVerifier
    {
        public static IAnalysisReport Report = AnalysisReports.ConstantKeyReport;

        [TestMethod]
        public void KeyMemberAssignment()
        {
            var test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            using (var aes = new AesManaged())
                aes.Key = new byte[]{12,12};
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 17)
                    }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void CreateEncryptorInvocation()
        {
            var test = @"
using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        private static readonly byte[] Test = new byte[] {123};

        public static void TestMethod()
        {
            using (var aes = new AesCryptoServiceProvider{Mode =  CipherMode.CBC})
            {
                aes.CreateEncryptor(Test, Test);
            } 
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 37)
                    }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void CreateDecryptorInvocation()
        {
            var test = @"
using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        private static readonly byte[] Test = new byte[] {123};

        public static void TestMethod()
        {
            using (var aes = new AesCryptoServiceProvider{Mode =  CipherMode.CBC})
            {
                aes.CreateDecryptor(Test, Test);
            } 
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 37)
                    }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void CreateInvalidInvocation()
        {
            var test = @"
using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        private static readonly byte[] Test = new byte[] {123};

        public static void TestMethod()
        {
            using (var aes = new AesCryptoServiceProvider{Mode =  CipherMode.CBC})
            {
                aes.Clear(Test, Test);
            } 
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConstantKeyAnalyzer();
        }
    }
}