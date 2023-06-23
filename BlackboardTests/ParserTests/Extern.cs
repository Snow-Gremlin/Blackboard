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
            "in int A = 3;").
            Perform();

        Assert.IsTrue(slate.HasNode("A"));
        slate.CheckValue(5, "A");
    }
}
