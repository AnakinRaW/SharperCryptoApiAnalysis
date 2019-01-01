using System;
using System.Security.Cryptography;

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass
    {
        public const string PasswordConst = "123";
        public static string PasswordStatic = "123";
        public string Password = "123", Test = "123";
        public readonly string PasswordReadonly = "123";
        public string PasswordProperty { get; } = "123";

        public static void TestMethod()
        {
            const string localConstPassword = "123";
            const string localPassword = "123";

            string passwordFine;
        }

        public static int Random()
        {
            var random = new Random();
            return random.Next();
        }

        public static void Encrypt()
        {
            using (var cipher = SymmetricAlgorithm.Create())
            {
                cipher.CreateEncryptor();

                cipher.Clear();
            }
        }

        public static byte[] GenerateKey()
        {
            var key = new byte[1];
            using (var randomGenerator = RandomNumberGenerator.Create())
            {
                randomGenerator.GetBytes(key);
                return key;
            }
        }

        public static void Hash()
        {
            using (var hash = HashAlgorithm.Create())
            {
            }
        }
    }
}

namespace SharperCryptoApiAnalysis.BaseAnalyzers.Tests
{
    internal class DraftClass2
    {
        public static void DeriveKey()
        {
            using (var pbkdf = new Rfc2898DeriveBytes("123", new byte[8]))
            {
            }
        }

        public static void Encrypt()
        {
            var keyAndIv = new byte[] { 123 };
            using (var aes = new AesCryptoServiceProvider{Mode =  CipherMode.ECB})
            {
                aes.IV = keyAndIv;
                aes.Key = keyAndIv;
            }     
        }

        public static byte[] GenerateKey(int size)
        {
            var key = new byte[size];
            using (var randomGenerator = new RNGCryptoServiceProvider())
            {
                randomGenerator.GetBytes(key);
                return key;
            }
        }
    }
}
