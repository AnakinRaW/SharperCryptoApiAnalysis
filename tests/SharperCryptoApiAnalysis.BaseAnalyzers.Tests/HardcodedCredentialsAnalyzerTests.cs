using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.BaseAnalyzers.Reports;
using SharperCryptoApiAnalysis.BaseAnalyzers.Tests.Verifiers;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    [TestClass]
    public class HardcodedCredentialsAnalyzer : CodeFixVerifier
    {
        public static IAnalysisReport Report = AnalysisReports.HardcodedCredentialsReprot;

        [TestMethod]
        public void ConstantField()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public const string PasswordConst = ""123"";
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 43)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void StaticField()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static string PasswordStatic = ""123"";
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 45)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void Field()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public string Password = ""123"";
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 32)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void FieldReadonly()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public readonly string Password = ""123"";
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 41)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void Property()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public string PasswordProperty { get; } = ""123"";
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 49)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void ConstLocalVariable()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            const string localConstPassword = ""123"";
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 45)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void LocalVariable()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            string localPassword = ""123"";
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 34)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void LocalVariableFlow()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            const string i = ""123"";
            string localPassword = i;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 34)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void LocalVariableFine()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            string localPassword;
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void HardCodedPasswordKDF()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            using (var pbkd2 = new Rfc2898DeriveBytes(""123"", 16, 10000))
            {
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
                        new DiagnosticResultLocation("Test0.cs", 9, 55)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void HardCodedPasswordKDFVariableByteInitializer()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            var pw = new[] {(byte) 123};
            using (var pbkd2 = new Rfc2898DeriveBytes(pw, 16, 10000))
            {
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
                        new DiagnosticResultLocation("Test0.cs", 10, 55)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void HardCodedPasswordKDFVariableByteNoInitializer()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            var pw = new byte[1];
            using (var pbkd2 = new Rfc2898DeriveBytes(pw, 16, 10000))
            {
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
                        new DiagnosticResultLocation("Test0.cs", 10, 55)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new Analyzers.HardcodedCredentialsAnalyzer();
        }
    }
}