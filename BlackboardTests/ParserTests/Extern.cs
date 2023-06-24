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
            "");
        // TODO: FIX. The problem is that A is set to the extern B but is not a child
        // so that when B is replaced, A still is the extern B. I need to make it so that
        // extern becomes a child of the new B, that way any copies of extern still use
        // extern B but extern B is updated automatically as a copy of B. The new B
        // takes the place of the old B. This then also helps allow disconnecting in the future.

        slate.Read(
            "in int C = 5;",
            "B := C;").
            Perform();
        slate.CheckValue(5, "A");
        slate.CheckValue(5, "B");
    }
}