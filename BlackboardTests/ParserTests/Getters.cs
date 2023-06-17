using Blackboard.Core;
using Blackboard.Core.Actions;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests;

[TestClass]
public class Getters {

    [TestMethod]
    public void TestBasicParses_TypedGetter() {
        Slate slate = new();
        Result result = slate.Read(
            "get int A = 3;",
            "get bool B = true;",
            "get {",
            "   double C = 4;",
            "   string D = 'Boom stick';",
            "}").
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
            "get {" +
            "   C = 4.0;",
            "   D = 'Boom stick';",
            "}").
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

        Result result = slate.Read(
            "get A;",
            "get B;",
            "get C;").
            Perform();
        result.CheckNames("A, B, C");
        result.CheckValue(3, "A");
        result.CheckValue(4, "B");
        result.CheckValue(7, "C");

        result = slate.Read(
            "get A, B, C;").
            Perform();
        result.CheckNames("A, B, C");
        result.CheckValue(3, "A");
        result.CheckValue(4, "B");
        result.CheckValue(7, "C");

        result = slate.Read(
            "get A, D = A*2.0 + B, B, E = 'Hello';").
            Perform();
        result.CheckNames("A, D, B, E");
        result.CheckValue(3, "A");
        result.CheckValue(4, "B");
        result.CheckValue(10.0, "D");
        result.CheckValue("Hello", "E");
    }

    [TestMethod]
    public void TestBasicParses_GetErrors() {
        Slate slate = new();
        slate.Read("in A = 3;").Perform();

        Tools.TestTools.CheckException(() =>
            slate.Read("get X;").Perform(),
            "Error occurred while parsing input code.",
            "[Error: No identifier found in the scope stack.",
            "   [Identifier: X]",
            "   [Location: Unnamed:1, 5, 5]]");

        Tools.TestTools.CheckException(() =>
            slate.Read("get int X = A*2.0;").Perform(),
            "Error occurred while parsing input code.",
            "[Error: The value type can not be cast to the given type.",
            "   [Location: Unnamed:1, 17, 17]",
            "   [Target: int]",
            "   [Type: double]",
            "   [Value: Mul<double>[0](Implicit<double>(Input<int>), <double>[2])]]");
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

    [TestMethod]
    public void TestFormula_VarGetter_CommaSeparated() {
        Slate slate = new();
        slate.Read(
            "in A = 3;",
            "in B = 2;").
            Perform();

        Result result = slate.Read(
            "get A, B, C = A + B;").
            Perform();
        result.CheckValue(3, "A");
        result.CheckValue(2, "B");
        result.CheckValue(5, "C");

        result = slate.Read(
            "get double A, B, C = (A + B)*2.0;").
            Perform();
        result.CheckValue(3.0, "A");
        result.CheckValue(2.0, "B");
        result.CheckValue(10.0, "C");

        result = slate.Read(
            "get string A, B, C = (A + B)*2.0;").
            Perform();
        result.CheckValue("3", "A");
        result.CheckValue("2", "B");
        result.CheckValue("10", "C");
    }
}
