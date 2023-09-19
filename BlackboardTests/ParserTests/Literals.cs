using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Record;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests;

[TestClass]
internal class Literals {

    static private void checkLit<T>(string input, T exp) {
        Slate slate = new();
        Result r = slate.Read("a := "+input+";").Perform();
        T value = (T)r.GetValueAsObject("a");
        Assert.AreEqual(exp, value, "For: "+input);
    }
    
    [TestMethod]
    public void TestLiteral_Zero() {
        checkLit("0", 0);
        checkLit("00", 0);
        checkLit("0_0", 0);
    }

}
