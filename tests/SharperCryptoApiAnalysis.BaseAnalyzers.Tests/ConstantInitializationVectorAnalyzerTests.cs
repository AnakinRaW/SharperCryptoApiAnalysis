using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.BaseAnalyzers.Analyzers;
using SharperCryptoApiAnalysis.BaseAnalyzers.CodeFixes;
using SharperCryptoApiAnalysis.BaseAnalyzers.Tests.Verifiers;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    [TestClass]
    public class ConstantInitializationVectorAnalyzerTests : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            using (var aes = new AesManaged())
                aes.IV = new byte[]{12,12};
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SCAA001",
                Message = "Compile-time constant IV",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 17)
                    }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestMethod3()
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
            {
                aes.IV = new[] {(byte) 123};
            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SCAA001",
                Message = "Compile-time constant IV",
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
        public void TestMethod4()
        {
            var test = @"
    using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        private static readonly byte[] Test = new[] {(byte) 123};

        public static void TestMethod()
        {
            using (var aes = new AesManaged())
            {
                aes.IV = Test;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "SCAA001",
                Message = "Compile-time constant IV",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestMethod5()
        {
            var test = @"
    using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        private static readonly byte[] Test = {123};

        public static void TestMethod()
        {
            using (var aes = new AesManaged())
            {
                var iv = Test;
                aes.IV = iv;
            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SCAA001",
                Message = "Compile-time constant IV",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 15, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestMethod6()
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
            using (var aes = new AesManaged())
            {
                var iv = Test;
                var iv2 = iv;
                aes.IV = iv2;
            }
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "SCAA001",
                Message = "Compile-time constant IV",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 16, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestMethod7()
        {
            var test = @"
using System;
using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            using (var aes = new AesManaged())
            {
                aes.IV = GenerateKey();
            }
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
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
            using (var aes = new AesManaged())
            {
                aes.IV = new byte[]{12,12};
            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "SCAA001",
                Message = "Compile-time constant IV",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void TestMethod()
        {
            using (var aes = new AesManaged())
            {
                aes.GenerateIV();
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
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
            using (var aes = new AesManaged())
            {
                aes.IV = Test();
            }
        }

        public static byte[] Test()
        {
            return new byte[] {123};
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "SCAA001",
                Message = "Compile-time constant IV",
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
        public void TestMethod10()
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
            {
                aes.IV = Test();
            }
        }

        public static byte[] Test()
        {
            var key = new byte[1];
            new Random().NextBytes(key);
            return key;
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestMethod11()
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
            {
                aes.IV = Test();
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
                Id = "SCAA001",
                Message = "Compile-time constant IV",
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
        public void CreateEncryptorTestNonConstant()
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
                aes.CreateEncryptor(aes.Key, aes.IV);
            } 
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void CreateEncryptorTestConstant()
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
                aes.CreateEncryptor(aes.Key, Test);
            } 
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "SCAA001",
                Message = "Compile-time constant IV",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 46)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConstantInitializationVectorAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ConstantInitializationVectorAnalyzerCodeFixProvider();
        }
    }
}