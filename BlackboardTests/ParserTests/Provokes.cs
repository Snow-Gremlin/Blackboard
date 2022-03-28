using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Provokes {

        [TestMethod]
        public void TestBasicParses_TypedInput() {
            /*
            TODO: Check Triggers and Provokes
            Slate slate = new();
            slate.Read(
                "in int A = 2, B = 3;",
                "in bool C = true;",
                "",
                "namespace D {",
                "   in double E = 3.14;",
                "   in int F, G;",
                "   in bool H;",
                "   in double I;",
                "}").
                Perform();

            slate.CheckValue(2, "A");
            slate.CheckValue(3, "B");
            slate.CheckValue(true, "C");
            slate.CheckValue(3.14, "D", "E");
            slate.CheckValue(0, "D", "F");
            slate.CheckValue(0, "D", "G");
            slate.CheckValue(false, "D", "H");
            slate.CheckValue(0.0, "D", "I");
            */
            Assert.Fail();
        }
    }
}
