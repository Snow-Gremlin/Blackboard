using Blackboard.Core;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests;

[TestClass]
public class Extern {

    [TestMethod]
    public void TestBasicParses_ExternToIn() {
        Slate slate = new();
        slate.Read(
            "extern int A = 2;",
            "extern int B = 3;",
            "in int A = 4;").
            Perform();
        Assert.IsTrue(slate.HasNode("A"));
        Assert.IsTrue(slate.HasNode("B"));
        slate.CheckValue(4, "A");
        slate.CheckValue(3, "B");

        slate.Read(
            "in int B = 5;",
            "A = 6;").
            Perform();
        slate.CheckValue(6, "A");
        slate.CheckValue(5, "B");
    }

    [TestMethod]
    public void TestBasicParses_ExternToRule() {
        Slate slate = new(addConsts: false);
        slate.Read(
            "extern int A = 2;",
            "extern int B = 3;",
            "A := B;").
            Perform();
        Assert.IsTrue(slate.HasNode("A"));
        Assert.IsTrue(slate.HasNode("B"));
        slate.CheckValue(3, "A");
        slate.CheckValue(3, "B");
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Shell<int>[3](B[3]),",
            "  B: Extern<int>[3]",
            "}");

        slate.Read(
            "in int C = 5;",
            "B := C;").
            Perform();
        slate.CheckValue(5, "A");
        slate.CheckValue(5, "B");
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Shell<int>[5](B),",
            "  B: Shell<int>[5](C[5]),",
            "  C: Input<int>[5]",
            "}");
    }

    [TestMethod]
    public void TestBasicParses_ExternToNamespace() {
        Slate slate = new(addConsts: false);
        TestTools.CheckException(() =>
            slate.Read(
                "extern int A;",
                "namespace A {",
                "  B := 8;",
                "}").
                Perform(),
            "Error occurred while parsing input code.",
            "[Error: Can not open namespace. Another non-namespace exists by that name.",
            "   [Identifier: A]",
            "   [Location: Unnamed:2, 11, 25]]");
    }

    // TODO: Add test for group define of extern and extern in namespaces
}