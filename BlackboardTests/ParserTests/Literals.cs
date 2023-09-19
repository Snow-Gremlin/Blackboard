using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Record;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.ParserTests;

[TestClass]
public class Literals {

    static private void checkLit<T>(string input, T exp) {
        Slate slate = new();
        Result r = slate.Read("get a = "+input+";").Perform();
        T? value = (T?)r.GetValueAsObject("a");
        Assert.AreEqual(exp, value, "For: "+input);
    }
    
    [TestMethod]
    public void TestLiteral_ZeroInt() {
        checkLit("0", 0);
        checkLit("00", 0);
        checkLit("0_0", 0);
        checkLit("0b", 0);
        checkLit("00b", 0);
        checkLit("0_0b", 0);
        checkLit("0o", 0);
        checkLit("00o", 0);
        checkLit("0_0o", 0);
        checkLit("0d", 0);
        checkLit("00d", 0);
        checkLit("0_0d", 0);
        checkLit("0x0", 0);
        checkLit("0x00", 0);
        checkLit("0x0_0", 0);
    }

}
