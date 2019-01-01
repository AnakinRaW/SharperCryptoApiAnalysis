using System.IO;
using System.Security.Cryptography;
using System.Text;
using Encryption;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharperCryptoApiAnalysis.Crypto.Tests
{
    [TestClass]
    public class CryptoClassTests
    {
        [TestMethod]
        public void SymmetricEncryption()
        {
            var secretMessage = "This is very secret";
            var cipher = SymmetricCryptoProvider.SimpleEncryptWithPassword(secretMessage, "123456789123", Encoding.UTF8);
            var clear = SymmetricCryptoProvider.SimpleDecryptWithPassword(cipher, "123456789123", Encoding.UTF8);
            Assert.AreEqual(secretMessage, clear);
        }


        [TestMethod]
        public void SymmetricEncryptionRaw()
        {
            byte[] key = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(key);


            var secretMessage = "This is very secret";
            var cipher = SymmetricCryptoProvider.EncryptMessage(secretMessage, key, Encoding.UTF8);
            var clear = SymmetricCryptoProvider.DecryptMessage(cipher, key, Encoding.UTF8);

            Assert.AreEqual(secretMessage, clear);
        }

        [TestMethod]
        public void SymmetricEncryptionFile()
        {
            var path = @"C:\Test\test.txt";

            var text = File.ReadAllText(path);
            SymmetricCryptoProvider.SimpleEncryptFileWithPassword(new FileInfo(path), "123456789123");
            SymmetricCryptoProvider.SimpleDecryptFileWithPassword(new FileInfo(path), "123456789123");

            var text2 = File.ReadAllText(path);

            Assert.AreEqual(text, text2);
        }


        [TestMethod]
        public void SymmetricEncryptionMac()
        {
            var secretMessage = "This is very secret";
            var cipher = SymmetricCryptoProviderWithMac.SimpleEncryptWithPassword(secretMessage, "123456789123", Encoding.UTF8);
            var clear = SymmetricCryptoProviderWithMac.SimpleDecryptWithPassword(cipher, "123456789123", Encoding.UTF8);
            Assert.AreEqual(secretMessage, clear);
        }

        [TestMethod]
        public void SymmetricEncryptionFileMac()
        {
            var path = @"C:\Test\test.txt";

            var text = File.ReadAllText(path);
            SymmetricCryptoProviderWithMac.SimpleEncryptFileWithPassword(new FileInfo(path), "123456789123");
            SymmetricCryptoProviderWithMac.SimpleDecryptFileWithPassword(new FileInfo(path), "123456789123");

            var text2 = File.ReadAllText(path);

            Assert.AreEqual(text, text2);
        }


        [TestMethod]
        public void SymmetricEncryptionRawMac()
        {
            byte[] key = new byte[16];
            byte[] auth = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(key);
            rng.GetBytes(auth);


            var secretMessage = "This is very secret";
            var cipher = SymmetricCryptoProviderWithMac.EncryptMessage(secretMessage, key, auth, Encoding.UTF8);
            var clear = SymmetricCryptoProviderWithMac.DecryptMessage(cipher, key, auth, Encoding.UTF8);

            Assert.AreEqual(secretMessage, clear);
        }
    }
}
