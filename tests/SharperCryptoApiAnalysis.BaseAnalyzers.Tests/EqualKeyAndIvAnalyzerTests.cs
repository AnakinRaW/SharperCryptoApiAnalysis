using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.BaseAnalyzers.Reports;
using SharperCryptoApiAnalysis.BaseAnalyzers.Tests.Verifiers;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    [TestClass]
    public class EqualKeyAndIvAnalyzerTests : CodeFixVerifier
    {
        public static IAnalysisReport Report = AnalysisReports.EqualKeyAndIvReport;

        [TestMethod]
        public void DifferentKeyAndIvAssignment()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Encrypt()
        {
            var key = new byte[] { 123 };
            var iv = new byte[] { 987 };
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.IV = iv;
                aes.Key = key;
            }     
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void GeneratingKeyAndIvIAssignment()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Encrypt()
        {
            var key = new byte[] { 123 };
            var iv = new byte[] { 987 };
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.GenerateIV();
                aes.GenerateKey();
            }     
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void SameKeyAndIvIAssignment()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Encrypt()
        {
            var keyAndIv = new byte[] { 123 };
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.IV = keyAndIv;
                aes.Key = keyAndIv;
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
                        new DiagnosticResultLocation("Test0.cs", 13, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void SameKeyAndIvIAssignmentDifferentVariables()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Encrypt()
        {
            var key = new byte[] { 123 };
            var iv = new byte[] { 123 };
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.IV = key;
                aes.Key = iv;
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
                        new DiagnosticResultLocation("Test0.cs", 14, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IvIsKeyAssignment()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Encrypt()
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.IV = aes.Key;
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
                        new DiagnosticResultLocation("Test0.cs", 11, 26)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void KeyIsIvAssignment()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Encrypt()
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Key = aes.IV;
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
                        new DiagnosticResultLocation("Test0.cs", 11, 27)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new Analyzers.EqualKeyAndIvAnalyzer();
        }
    }
}