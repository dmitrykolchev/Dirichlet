﻿using System.Numerics;
using Decompose.Numerics;
using Decompose.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decompose.Scripting.Tests
{
    [TestClass]
    public class DecomposeTests
    {
        [TestMethod]
        public void ScriptingTests()
        {
            Assert.AreEqual((Rational)3, Evaluate("1+2"));
            Assert.AreEqual(3, Evaluate("int 1 + int 2"));
            Assert.AreEqual(true, Evaluate("5 2 == 4(-5) (mod 6)"));
            Assert.AreEqual((Rational)5, Evaluate("a=2; b=3; n=17; a+b (mod n)"));
            Assert.AreEqual((Rational)16, Evaluate("a=2; b=3; n=17; a-b (mod n)"));
            Assert.AreEqual((Rational)6, Evaluate("a=2; b=3; n=17; a*b (mod n)"));
            Assert.AreEqual((Rational)12, Evaluate("a=2; b=3; n=17; a/b (mod n)"));
            Assert.AreEqual((Rational)8, Evaluate("a=2; b=3; n=17; a^b (mod n)"));
            Assert.AreEqual((Rational)6, Evaluate("a=2; b=3; n=17; a^(1/2) (mod n)"));
            Assert.AreEqual(Rational.Parse("1606938044258990275541962092341162602522202993782792835301375/8796093022207"),
                Evaluate("((1<<200)-1)/((1<<43)-1)"));
            Assert.AreEqual(Rational.Parse("47021221151697033430710241825459573109"), Evaluate("(47021221151697033430710241825459573109^3)^(1/3)"));
        }

        private object Evaluate(string text)
        {
            var engine = new Engine();
            return new Parser().Compile(engine, CodeType.Script, text).Root.Get(engine);
        }
    }
}
