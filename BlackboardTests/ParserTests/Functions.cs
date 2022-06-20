using Blackboard.Core;
using Blackboard.Core.Extensions;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Functions {

        [TestMethod]
        public void CheckAllFunctionsAreTested() {
            HashSet<string> testedTags = TestTools.TestTags(typeof(Functions)).ToHashSet();
            HashSet<string> funcTags = TestTools.FuncDefTags(new Slate().Global).
                WhereNot(tag => tag.StartsWith(Slate.OperatorNamespace)).ToHashSet();

            List<string> notTested = funcTags.WhereNot(testedTags.Contains).ToList();
            List<string> notAnFunc = testedTags.WhereNot(funcTags.Contains).ToList();

            if (notAnFunc.Count > 0 || notTested.Count > 0) {
                Assert.Fail("Tests do not match the existing function:\n" +
                    "Not Tested (" + notTested.Count + "):\n  " + notTested.Join("\n  ") + "\n" +
                    "Not a Function (" + notAnFunc.Count + "):\n  " + notAnFunc.Join("\n  "));
            }
        }
    }
}
