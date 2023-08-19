using Blackboard.Core.Formula;
using Blackboard.Core.Record;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace BlackboardTests.BlackboardTests;

[TestClass]
public class BlackboardTests {

    [TestMethod]
    public void TestOnProvoke() {
        Blackboard.Blackboard b = new();
        b.Perform("in trigger x;");
        StringBuilder buf = new();
        
        ITriggerWatcher x = b.OnProvoke("x");
        x.OnProvoked += (object sender, EventArgs e) => buf.Append("X");

        ITriggerWatcher y = b.OnProvoke("y");
        y.OnProvoked += (object sender, EventArgs e) => buf.Append("Y");
        
        ITriggerWatcher z = b.OnProvoke("z");
        z.OnProvoked += (object sender, EventArgs e) => buf.Append("Z");

        buf.Check("");
        Formula provokeX = b.CreateFormula("-> x;");
        buf.Check("");
        provokeX.Perform();
        buf.Check("X");

        b.Perform("in trigger y;");
        buf.Check("");
        Formula provokeY = b.CreateFormula("-> y;");
        buf.Check("");
        provokeY.Perform();
        buf.Check("Y");

        provokeX.Perform();
        buf.Check("X");
        provokeY.Perform();
        buf.Check("Y");
        provokeX.Perform();
        provokeY.Perform();
        buf.Check("XY");
        
        b.Perform("z := x & y;");
        buf.Check("");
        provokeX.Perform();
        buf.Check("X");
        provokeY.Perform();
        buf.Check("Y");
        provokeX.Perform();
        provokeY.Perform();
        buf.Check("XY"); // No 'Z' because they are performed separately.

        Formula provokeZ = Formula.Join(provokeX, provokeY);
        provokeZ.Perform();
        buf.Check("XYZ");
    }
}
