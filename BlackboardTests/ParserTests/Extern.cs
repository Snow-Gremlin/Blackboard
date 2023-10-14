using Blackboard.Core;
using Blackboard.Core.Record;
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
            "[Error: Error parsing namespace",
            "   [Location: Unnamed:2, 11, 25]",
            "   [Error: Can not open namespace. Another non-namespace exists by that name.",
            "      [Identifier: A]]]");
    }

    [TestMethod]
    public void TestBasicParses_ExternInNamespaces() {
        Slate slate = new(addConsts: false);
        slate.Read(
            "namespace A {",
            "   extern int B = 2;",
            "   extern int C = 3;",
            "}",
            "D := A.B;",
            "namespace A {",
            "   B := C;",
            "}").
            Perform();
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Namespace{",
            "    B: Shell<int>[3](A.C[3]),",
            "    C: Extern<int>[3]",
            "  },",
            "  D: Shell<int>[3](A.B)",
            "}");

        slate.Read(
            "namespace A {",
            "   C := 8;",
            "}").
            Perform();
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Namespace{",
            "    B: Shell<int>[8](A.C[8]),",
            "    C: Literal<int>[8]",
            "  },",
            "  D: Shell<int>[8](A.B)",
            "}");
    }

    [TestMethod]
    public void TestBasicParses_ExternAroundNamespaces() {
        Slate slate = new(addConsts: false);
        slate.Read(
            "extern {",
            "   namespace A {",
            "      int B = 2;",
            "      int C = 3;",
            "   }",
            "}",
            "D := A.B;",
            "define {",
            "   namespace A {",
            "      B = C;",
            "   }",
            "}").
            Perform();
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Namespace{",
            "    B: Shell<int>[3](A.C[3]),",
            "    C: Extern<int>[3]",
            "  },",
            "  D: Shell<int>[3](A.B)",
            "}");

        slate.Read(
            "namespace A {",
            "   C := 8;",
            "}").
            Perform();
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Namespace{",
            "    B: Shell<int>[8](A.C[8]),",
            "    C: Literal<int>[8]",
            "  },",
            "  D: Shell<int>[8](A.B)",
            "}");
    }

    [TestMethod]
    public void TestBasicParses_ExternGroup() {
        Slate slate = new(addConsts: false);
        slate.Read(
            "extern {",
            "   int A = 2;",
            "   int B = 3;",
            "}",
            "B := A;",
            "in A = 9;").
            Perform();
        slate.CheckGraphString(
            "Global: Namespace{",
            "  A: Input<int>[9],",
            "  B: Shell<int>[9](A[9])",
            "}");
    }

    [TestMethod]
    public void TestBasicParses_ExternCycle() {
        Slate slate = new(addConsts: false);
        slate.Read(
            "extern int A = 3;",
            "B := A*3 + 2;").
            Perform();

        TestTools.CheckException(() =>
            slate.Read(
                "A := B - 1;").
                Perform(),
            "May not add children to a parent which would cause a loop",
            "[parent: Sub<int>[0](Sum<int>(Mul<int>, <int>[2]), <int>[1])]",
            "[children: Shell<int>[0](Sub<int>(Sum<int>, <int>[1]))]");
    }
}