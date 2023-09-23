using Blackboard.Core;
using Blackboard.Core.Record;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests;

[TestClass]
public class Temps {

    [TestMethod]
    public void TestBasicParses_ConstantTypedTemp() {
        Slate slate = new();
        slate.Read(
            "temp int A = 2;",
            "temp int B = 3;",
            "int C := A + B;").
            Perform();

        Assert.IsFalse(slate.HasNode("A"));
        Assert.IsFalse(slate.HasNode("B"));
        Assert.IsTrue(slate.HasNode("C"));
        slate.CheckValue(5, "C");
    }

    [TestMethod]
    public void TestBasicParses_VarTempWithInput() {
        Slate slate = new(addConsts: false);
        slate.Read(
            "in int A = 1;",
            "temp B = A * 2 + 3;",
            "C := B + 5;",
            "D := B * 4;").
            Perform();
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Input<int>[1],",
            "  C: Sum<int>[10](Sum<int>(Mul<int>(A[1], <int>[2]), <int>[3]), <int>[5]),",
            "  D: Mul<int>[20](Sum<int>(Mul<int>(A[1], <int>[2]), <int>[3]), <int>[4])",
            "}");

        Assert.IsFalse(slate.HasNode("B"));
        slate.CheckValue(10, "C");
        slate.CheckValue(20, "D");

        slate.SetInt(2, "A");
        slate.CheckEvaluate(
            "Start Eval (pending: 1)",
            "  Evaluated (changed: True, depth: 1, node: Mul<int>[4](A, <int>[2]), remaining: 1)",
            "  Evaluated (changed: True, depth: 2, node: Sum<int>[7](Mul<int>(A, <int>[2]), <int>[3]), remaining: 2)",
            "  Evaluated (changed: True, depth: 3, node: D: Mul<int>[28](Sum<int>(Mul<int>, <int>[3]), <int>[4]), remaining: 1)",
            "  Evaluated (changed: True, depth: 3, node: C: Sum<int>[12](Sum<int>(Mul<int>, <int>[3]), <int>[5]), remaining: 0)",
            "End Eval ()");
        slate.CheckValue(12, "C");
        slate.CheckValue(28, "D");
    }
    
    [TestMethod]
    public void TestBasicParses_TempGroup() {
        Slate slate = new();
        slate.Read(
            "in A = 3;",
            "temp {",
            "   int B = A*2;",
            "   C = A*3;",
            "}",
            "int D := B + C;").
            Perform();

        Assert.IsFalse(slate.HasNode("B"));
        Assert.IsFalse(slate.HasNode("C"));
        slate.CheckValue(15, "D");
    }

    // TODO: Add namespace tests
}
