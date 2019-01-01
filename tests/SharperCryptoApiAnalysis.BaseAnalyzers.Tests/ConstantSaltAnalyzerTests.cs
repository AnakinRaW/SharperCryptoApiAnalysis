using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers;
using SharperCryptoApiAnalysis.BaseAnalyzers.Tests.Verifiers;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    [TestClass]
    public class ConstantSaltAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestMethod2()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 1))
            {

            }
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }


        [TestMethod]
        public void TestMethod3()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", new byte[]{123}))
            {

            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA010",
                Message = "Compile-time constant Salt",
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
        public void TestMethod4()
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
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", i))
            {

            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SCAA010",
                Message = "Compile-time constant Salt",
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
        public void TestMethod5()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        private byte[] _i = new byte[]{123};
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", _i))
            {

            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SCAA010",
                Message = "Compile-time constant Salt",
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
        public void TestMethod6()
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
            var j = i;
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", j))
            {

            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA010",
                Message = "Compile-time constant Salt",
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
        public void TestMethod7()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 1))
            {
                pbkdf.Salt = new[] {(byte) 123};
            }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "SCAA010",
                Message = "Compile-time constant Salt",
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
        public void TestMethod8()
        {
            var test = @"
using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 1))
            {
                pbkdf.Salt = Test();
            }
        }

        public static byte[] Test()
        {
            if (true)
                return null;
            var test = new[] {(byte)123}
            return test;
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "SCAA010",
                Message = "Compile-time constant Salt",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestMethod9()
        {
            var test = @"
using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", Test()))
            {
            }
        }

        public static byte[] Test()
        {
            if (true)
                return null;
            var test = new[] {(byte)123}
            return test;
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "SCAA010",
                Message = "Compile-time constant Salt",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 62)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConstantSaltAnalyzer();
        }
    }
}