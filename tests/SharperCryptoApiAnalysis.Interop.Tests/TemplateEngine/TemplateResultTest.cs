using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharperCryptoApiAnalysis.Interop.TemplateEngine;

namespace SharperCryptoApiAnalysis.Interop.Tests.TemplateEngine
{
    [TestClass]
    public class TemplateResultTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var template = "$test$keintest$123$";
            var te = new SimpleTemplateEngine(template);
            te.AddReplacementValue("test", "System");
            te.AddReplacementValue("123", "TestClass");

            Assert.AreEqual("SystemkeintestTestClass", te.GetResult());
        }
    }
}
