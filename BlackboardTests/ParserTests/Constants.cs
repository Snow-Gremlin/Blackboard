using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Constants {

        [TestMethod]
        public void CheckAllConstantsAreTested() {
            HashSet<string> testedTags = TestTools.TestTags(typeof(Constants)).ToHashSet();
            HashSet<string> constTags = TestTools.ConstTags(new Slate().Global).ToHashSet();

            List<string> notTested = constTags.WhereNot(testedTags.Contains).ToList();
            List<string> notAnConst = testedTags.WhereNot(constTags.Contains).ToList();

            if (notAnConst.Count > 0 || notTested.Count > 0) {
                Assert.Fail("Tests do not match the existing constant:\n" +
                    "Not Tested (" + notTested.Count + "):\n  " + notTested.Join("\n  ") + "\n" +
                    "Not a Constant (" + notAnConst.Count + "):\n  " + notAnConst.Join("\n  "));
            }
        }

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
