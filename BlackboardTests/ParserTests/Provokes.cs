using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Provokes {

        [TestMethod]
        public void TestBasicParses_TypedInput() {
            Slate slate = new();
            slate.Read(
                "in trigger A, B;",
                "C := A | B;").
                Perform();

            //slate.CheckValue(2, "A");
            //slate.CheckValue(3, "B");
            //slate.CheckValue(true, "C");
            //slate.CheckValue(3.14, "D", "E");
            //slate.CheckValue(0, "D", "F");
            //slate.CheckValue(0, "D", "G");
            //slate.CheckValue(false, "D", "H");
            //slate.CheckValue(0.0, "D", "I");
        }
    }
}
