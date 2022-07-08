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
                TestTools.TestTags(this.GetType()),
                "Tests do not match the existing constant");

        /// <summary>
        /// Checks if the given double constant name can be gotten from a new slate
        /// and it matches the expected value.
        /// </summary>
        /// <param name="name">The name of the double constant with any required namespaces</param>
        /// <param name="exp">The expected value to compare against.</param>
        static private void checkDouble(string name, double exp) =>
            new Slate().Read("get A = " + name + ";").Perform().CheckValue(exp, "A");

        /// <summary>
        /// Checks if the given constant name can be gotten from a new slate
        /// and it matches the expected value.
        /// </summary>
        /// <param name="name">The name of the constant with any required namespaces</param>
        /// <param name="exp">The expected value to compare against.</param>
        static private void checkObject(string name, object exp) =>
            new Slate().Read("get A = " + name + ";").Perform().CheckObject(exp, "A");

        [TestMethod]
        [TestTag("e")]
        public void TestConstants_e() => checkDouble("e", S.Math.E);

        [TestMethod]
        [TestTag("pi")]
        public void TestOperators_pi() => checkDouble("pi", S.Math.PI);

        [TestMethod]
        [TestTag("tau")]
        public void TestOperators_tau() => checkDouble("tau", S.Math.Tau);

        [TestMethod]
        [TestTag("sqrt2")]
        public void TestOperators_sqrt2() => checkDouble("sqrt2", S.Math.Sqrt(2.0));

        [TestMethod]
        [TestTag("inf")]
        public void TestOperators_inf() => checkDouble("inf", double.PositiveInfinity);

        [TestMethod]
        [TestTag("nan")]
        public void TestOperators_nan() => checkDouble("nan", double.NaN);

        [TestMethod]
        [TestTag("null")]
        public void TestOperators_null() => checkObject("null", null);
    }
}
