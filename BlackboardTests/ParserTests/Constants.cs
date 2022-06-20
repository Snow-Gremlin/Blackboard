using Blackboard.Core;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S = System;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Constants {

        [TestMethod]
        public void CheckAllConstantsAreTested() =>
            TestTools.SetEntriesMatch(
                TestTools.ConstTags(new Slate().Global),
                TestTools.TestTags(typeof(Constants)),
                "Tests do not match the existing constant");

        /// <summary>
        /// Checks if the given constant name can be gotten from a new slate
        /// and it matches the expected value.
        /// </summary>
        /// <param name="name">The name of the constant with any required namespaces</param>
        /// <param name="exp">The expected value to compare against.</param>
        static private void checkConstant(string name, double exp) =>
            new Slate().Read("get A = " + name + ";").Perform().CheckValue(exp, "A");

        [TestMethod]
        [TestTag("e")]
        public void TestConstants_e() => checkConstant("e", S.Math.E);

        [TestMethod]
        [TestTag("pi")]
        public void TestOperators_pi() => checkConstant("pi", S.Math.PI);

        [TestMethod]
        [TestTag("tau")]
        public void TestOperators_tau() => checkConstant("tau", S.Math.Tau);

        [TestMethod]
        [TestTag("sqrt2")]
        public void TestOperators_and_And() => checkConstant("sqrt2", S.Math.Sqrt(2.0));
    }
}
