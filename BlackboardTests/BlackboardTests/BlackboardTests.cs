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
        StringBuilder buf = new();
        Blackboard.Blackboard b = new();
        b.Perform("in trigger x;");
        
        ITriggerWatcher x = b.OnProvoke("x");
        x.OnProvoked += (object sender, EventArgs e) => buf.Append('X');

        ITriggerWatcher y = b.OnProvoke("y");
        y.OnProvoked += (object sender, EventArgs e) => buf.Append('Y');
        
        ITriggerWatcher z = b.OnProvoke("z");
        z.OnProvoked += (object sender, EventArgs e) => buf.Append('Z');

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

    [TestMethod]
    public void TestOnChanged() {
        StringBuilder buf = new();
        Blackboard.Blackboard b = new();
        b.Perform("in int x;");
        
        IValueWatcher<int> x = b.OnChange<int>("x");
        x.OnChanged += (object sender, ValueEventArgs<int> e) => buf.Append("X(" + e.Previous + " -> " + e.Current + ")");

        IValueWatcher<int> y = b.OnChange<int>("y");
        y.OnChanged += (object sender, ValueEventArgs<int> e) => buf.Append("Y(" + e.Previous + " -> " + e.Current + ")");
        
        IValueWatcher<double> z = b.OnChange<double>("z");
        z.OnChanged += (object sender, ValueEventArgs<double> e) => buf.Append("Z(" + e.Previous + " -> " + e.Current + ")");

        buf.Check("");
        Formula setX1 = b.CreateFormula("x = 12;");
        Formula setX2 = b.CreateFormula("x = 34;");
        buf.Check("");
        setX1.Perform();
        buf.Check("X(0 -> 12)");
        setX2.Perform();
        buf.Check("X(12 -> 34)");
        setX1.Perform();
        buf.Check("X(34 -> 12)");
        
        b.Perform("in int y = 9;");
        buf.Check("Y(0 -> 9)");
        Formula setY1 = b.CreateFormula("y = 56;");
        Formula setY2 = b.CreateFormula("y = 78;");
        buf.Check("");
        setY1.Perform();
        buf.Check("Y(9 -> 56)");
        setY2.Perform();
        buf.Check("Y(56 -> 78)");
        setY1.Perform();
        buf.Check("Y(78 -> 56)");
        
        b.Perform("z := x + y/100.0;");
        buf.Check("Z(0 -> 12.56)");
        setX2.Perform();
        setY2.Perform();
        buf.Check("X(12 -> 34)Z(12.56 -> 34.56)Y(56 -> 78)Z(34.56 -> 34.78)");
        
        Formula set1 = Formula.Join(setX1, setY1);
        set1.Perform();
        buf.Check("X(34 -> 12)Z(34.78 -> 12.56)Y(78 -> 56)");
    }
}
