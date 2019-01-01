using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers;
using SharperCryptoApiAnalysis.BaseAnalyzers.Tests.Verifiers;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    [TestClass]
    public class LowCostFactorAnalyzer : CodeFixVerifier
    {
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void IterationCountCtorEmpty()
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
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 54)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IterationCountCtorTooLowLiteral()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 8, 1000))
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 65)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IterationCountCtorTooLowBcl()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 8, int.MinValue))
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 65)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IterationCountCtorTooLowVariable()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            var i = 1000;
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 8, i))
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 65)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IterationCountCtorTooLowVariableFlow()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            var i = 1000;
            var j = i;
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 8, j))
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 65)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IterationCountCtorTooLowMethod()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 8, GetInt()))
            {
            }
        }

        public static int GetInt()
        {
            return 1000;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 65)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IterationCountCtorMethod()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 8, GetInt()))
            {
            }
        }

        public static int GetInt()
        {
            return 10000;
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void IterationCountCtorLiteral()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 8, 10000))
            {
            }
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void IterationCountPropertyTooLowLiteral()
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
                pbkdf.IterationCount = 1000;
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IterationCountPropertyTooLowLiteralNoUsing()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            var pbkdf = new Rfc2898DeriveBytes(""123"", 8);
            pbkdf.IterationCount = 1000;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IterationCountCtorTooLowLiteralNoUsing()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            var pbkdf = new Rfc2898DeriveBytes(""123"", 8);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 47)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void IterationCountCtorTooLowWithMemeberAccess()
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
                var count = pbkdf.IterationCount;
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA011",
                Message = "Low iteration count for password derive function",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 54)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new Analyzers.LowCostFactorAnalyzer();
        }
    }
}