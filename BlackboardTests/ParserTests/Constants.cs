using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Constants {

        [TestMethod]
        public void CheckAllConstantsAreTested() {
            HashSet<string> testedTags = TestTools.TestTags(typeof(Constants));

            HashSet<string> constTags = new();
            Slate slate = new();
            foreach (KeyValuePair<string, INode> pair in slate.Global.Fields) {
                if (pair.Value is IConstant constant) constTags.Add(pair.Key);
            }

            List<string> notTested = constTags.WhereNot(testedTags.Contains).ToList();
            List<string> notAnConst = testedTags.WhereNot(constTags.Contains).ToList();

            if (notAnConst.Count > 0 || notTested.Count > 0) {
                Assert.Fail("Tests do not match the existing constant:\n" +
                    "Not Tested (" + notTested.Count + "):\n  " + notTested.Join("\n  ") + "\n" +
                    "Not a Constant (" + notAnConst.Count + "):\n  " + notAnConst.Join("\n  "));
            }
        }

        [TestMethod]
        [TestTag("e")]
        public void TestConstants_e() {
            Result result = new Slate().Read("get A = e;").Perform();
            result.CheckValue(System.Math.E, "A");
        }

        [TestMethod]
        [TestTag("pi")]
        public void TestOperators_pi() {
            Result result = new Slate().Read("get A = pi;").Perform();
            result.CheckValue(System.Math.PI, "A");
        }

        [TestMethod]
        [TestTag("tau")]
        public void TestOperators_tau() {
            Result result = new Slate().Read("get A = tau;").Perform();
            result.CheckValue(System.Math.Tau, "A");
        }

        [TestMethod]
        [TestTag("sqrt2")]
        public void TestOperators_and_And() {
            Result result = new Slate().Read("get A = sqrt2;").Perform();
            result.CheckValue(System.Math.Sqrt(2.0), "A");
        }
    }
}
