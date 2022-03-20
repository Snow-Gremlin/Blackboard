using Blackboard.Core;
using Blackboard.Core.Actions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests {

    [TestClass]
    public class Getters {

        [TestMethod]
        public void TestBasicParses_TypedGetter() {
            Slate slate = new();
            Result result = slate.Read(
                "get int A = 3;",
                "get bool B = true;",
                "get double C = 4;",
                "get string D = 'Boom stick';").
                Perform();

            result.CheckValue(3, "A");
            result.CheckValue(true, "B");
            result.CheckValue(4.0, "C");
            result.CheckValue("Boom stick", "D");
        }
        
        [TestMethod]
        public void TestBasicParses_VarGetter() {
            Slate slate = new();
            Result result = slate.Read(
                "get A = 3;",
                "get B = true;",
                "get C = 4.0;",
                "get D = 'Boom stick';").
                Perform();

            result.CheckValue(3, "A");
            result.CheckValue(true, "B");
            result.CheckValue(4.0, "C");
            result.CheckValue("Boom stick", "D");
        }
    }
}
