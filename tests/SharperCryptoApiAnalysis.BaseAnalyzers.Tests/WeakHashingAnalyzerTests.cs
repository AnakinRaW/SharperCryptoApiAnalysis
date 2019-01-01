using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.BaseAnalyzers.Reports;
using SharperCryptoApiAnalysis.BaseAnalyzers.Tests.Verifiers;
using SharperCryptoApiAnalysis.Interop.CodeAnalysis;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    [TestClass]
    public class WeakHashingAnalyzerTests : CodeFixVerifier
    {
        public static IAnalysisReport Report = AnalysisReports.WeakHashingReport;

        [TestMethod]
        public void UsingMd5Ctor()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = new MD5CryptoServiceProvider())
            {
            }
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
                        new DiagnosticResultLocation("Test0.cs", 9, 31)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [TestMethod]
        public void UsingHmacSha1Ctor()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = new HMACSHA1())
            {
            }
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void UsingHmacSha512Ctor()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = new HMACSHA512())
            {
            }
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void UsingSha256Factory()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = SHA256.Create())
            {
            }
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void UsingSha256FactorySpecific()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = SHA256.Create(""SHA256Managed""))
            {
            }
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }


        [TestMethod]
        public void UsingSha1Factory()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = SHA1.Create())
            {
            }
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
                        new DiagnosticResultLocation("Test0.cs", 9, 31)
                    }
            };


            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void UsingFactory()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = HashAlgorithm.Create())
            {
            }
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
                        new DiagnosticResultLocation("Test0.cs", 9, 31)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void UsingFactorySha1Parameter()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = HashAlgorithm.Create(""SHA1""))
            {
            }
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
                        new DiagnosticResultLocation("Test0.cs", 9, 31)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void UsingFactoryHMAC()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = KeyedHashAlgorithm.Create())
            {
            }
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
                        new DiagnosticResultLocation("Test0.cs", 9, 31)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void UsingFactoryHMACMd5()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = HashAlgorithm.Create(""HMACMD5""))
            {
            }
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
                        new DiagnosticResultLocation("Test0.cs", 9, 31)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [TestMethod]
        public void UsingFactoryMd5Variable()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            var md5 = ""MD5"";
            using (var hash = HashAlgorithm.Create(md5))
            {
            }
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
                        new DiagnosticResultLocation("Test0.cs", 10, 31)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void UsingFactoryMd5Method()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = HashAlgorithm.Create(GetHash()))
            {
            }
        }

        public static string GetHash()
        {
            if (true)
                return ""MD5"";
            return null;
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
                        new DiagnosticResultLocation("Test0.cs", 9, 31)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void UsingFactorySha512Method()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = HashAlgorithm.Create(GetHash()))
            {
            }
        }

        public static string GetHash()
        {
            if (true)
                return ""SHA512"";
            return null;
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void UsingHmacMd5Factory()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            using (var hash = HashAlgorithm.Create(""HMACMD5""))
            {
            }
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
                        new DiagnosticResultLocation("Test0.cs", 9, 31)
                    }
            };


            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void PBKDF2Md5()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 128, 10000, HashAlgorithmName.MD5))
            {
            }
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
                        new DiagnosticResultLocation("Test0.cs", 9, 32)
                    }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void PBKDF2Sha1()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes(""123"", 128, 10000))
            {
            }
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void Md5Hash()
        {
            string test = @"
using System.Security.Cryptography;
namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public static void Hash()
        {
            var hashProvider = new MD5CryptoServiceProvider();
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new Analyzers.WeakHashingAnalyzer();
        }
    }
}