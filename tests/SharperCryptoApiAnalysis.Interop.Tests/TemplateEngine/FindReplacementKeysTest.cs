using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.Interop.TemplateEngine;

namespace SharperCryptoApiAnalysis.Interop.Tests.TemplateEngine
{
    [TestClass]
    public class FindReplacementKeysTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var template = "$test$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys.Count;
            Assert.AreEqual(1, c);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var template = "$test$$test$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys.Count;
            Assert.AreEqual(1, c);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var template = "$test$$123$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys.Count;
            Assert.AreEqual(2, c);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var template = "$test$$123$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys;
            Assert.AreEqual("test", c[0]);
            Assert.AreEqual("123", c[1]);
        }

        [TestMethod]
        public void TestMethod5()
        {
            var template = "$test$keintest$123$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys;
            Assert.AreEqual("test", c[0]);
            Assert.AreEqual("123", c[1]);
        }

        [TestMethod]
        public void TestMethod6()
        {
            var template = "$test$keintest$123$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys.Count;
            Assert.AreEqual(2, c);
        }

        [TestMethod]
        public void TestMethod7()
        {
            var template = "$$test$keintest$123$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys.Count;
            Assert.AreEqual(2, c);
        }

        [TestMethod]
        public void TestMethod8()
        {
            var template = "$$test$keintest$123$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys;
            Assert.AreEqual("test", c[0]);
            Assert.AreEqual("123", c[1]);
        }

        [TestMethod]
        public void TestMethod9()
        {
            var template = "$$test$keintest$123$$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys.Count;
            Assert.AreEqual(2, c);
        }

        [TestMethod]
        public void TestMethod10()
        {
            var template = "$test$keintest$123$$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys;
            Assert.AreEqual("test", c[0]);
            Assert.AreEqual("123", c[1]);
        }

        [TestMethod]
        public void TestMethod11()
        {
            var template = "$$test$keintest$123$$$$$$$$";
            var engine = new SimpleTemplateEngine(template);
            var c = engine.ReplacementKeys.Count;
            Assert.AreEqual(2, c);
        }
    }
}
