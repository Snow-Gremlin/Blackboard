using Blackboard.Core;
using Blackboard.Core.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Optimization {

        [TestMethod]
        public void TestParseOptimization_Constants() {
            Slate slate = new();
            Formula formula = slate.LogRead(
                "in A = 21.0 + (1.0 + 3.0*2.0)*3.0;");
            formula.Check();


        }
    }
}
