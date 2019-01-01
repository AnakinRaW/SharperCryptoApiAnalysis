using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers;
using SharperCryptoApiAnalysis.BaseAnalyzers.Reports;
using SharperCryptoApiAnalysis.BaseAnalyzers.Tests.Verifiers;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    [TestClass]
    public class ShortSaltSizeAnalyzerTests : CodeFixVerifier
    {

        private static readonly IAnalysisReport Report = AnalysisReports.ShortSaltSizeReport;

        [TestMethod]
        public void SaltSizeOk()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 16))
            {
            }
        }
    }
}";
        }

        [TestMethod]
        public void SaltSizeTooShort()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 8))
            {
            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 62)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

        }

        [TestMethod]
        public void SaltSizeTooShortVariable()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            var int i = 8;
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", i))
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 62)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void SaltSizeTooShortBcl()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", int.MinValue))
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 62)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void SaltSizeTooShortFlow()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            var i = 8;
            var j = i;
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", j))
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 62)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void SaltSizeTooShortMethod()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", GetInt()))
            {
            }
        }

        public static int GetInt()
        {
            return 8;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 62)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void SaltSizeTooShortArray()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", new[] {(byte) 123}))
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 62)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void SaltSizeTooShortArrayVariable()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            var i = new byte[]{123};
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", i)
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 62)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void SaltSizeTooShortArrayVariableEmpty()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            var i = new byte[8];
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", i)
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 62)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void SaltSizeTooShortMemberArrayVariableEmpty()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            var i = new byte[8];
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", new byte[8])
            {
                pbkdf.Salt = i;
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = Report.Id,
                Message = Report.Summary,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ShortSaltSizeAnalyzer();
        }
    }
}