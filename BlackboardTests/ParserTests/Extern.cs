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
            "int A = 3;").
            Perform();

        Assert.IsFalse(slate.HasNode("A"));
        Assert.IsFalse(slate.HasNode("B"));
        Assert.IsTrue(slate.HasNode("C"));
        slate.CheckValue(5, "C");
    }
}
