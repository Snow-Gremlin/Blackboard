using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Functions {

        [TestMethod]
        public void CheckAllFunctionsAreTested() {
            HashSet<string> testedTags = TestTools.TestTags(typeof(Functions));

            HashSet<string> funcTags = new();
            Slate slate = new();
            foreach (KeyValuePair<string, INode> pair in slate.Global.Fields) {
                if (pair.Value is IFuncGroup group) {
                    foreach (IFuncDef def in group.Definitions)
                        funcTags.Add(pair.Key+":"+def.ReturnType.FormattedTypeName());
                }
            }

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
