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

        [TestMethod]
        public void TestBasicParses_GetExisting() {
            Slate slate = new();
            slate.Read(
                "in A = 3;",
                "B := 4;",
                "C := A + B;").
                Perform();

            // TODO: Add Comma (e.g. 'get A, B, C;') to all other getters
            // TODO: Test error for the ID not existing
            Result result = slate.Read(
                "get A;",
                "get B;",
                "get C;"). 
                Perform();

            result.CheckValue(3, "A");
            result.CheckValue(4, "B");
            result.CheckValue(7, "C");
        }

        [TestMethod]
        public void TestFormula_VarGetter() {
            Slate slate = new();
            slate.Read(
                "in A = 3;",
                "in B = 2;").
                Perform();
            Formula formula = slate.Read(
                "get A;",
                "get B = A + B;");

            Result result = formula.Perform();
            result.CheckValue(3, "A");
            result.CheckValue(5, "B");

            // Re-use formula
            slate.Read(
                "A = 5;",
                "B = 8;").
                Perform();
            result = formula.Perform();
            result.CheckValue(5, "A");
            result.CheckValue(13, "B");
        }
    }
}
