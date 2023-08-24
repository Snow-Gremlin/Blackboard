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
        x.OnProvoked += (object? sender, EventArgs e) => buf.Add("X");

        ITriggerWatcher y = b.OnProvoke("y");
        y.OnProvoked += (object? sender, EventArgs e) => buf.Add("Y");
        
        ITriggerWatcher z = b.OnProvoke("z");
        z.OnProvoked += (object? sender, EventArgs e) => buf.Add("Z");

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
        buf.Check("X, Y");
        
        b.Perform("z := x & y;");
        buf.Check("");
        provokeX.Perform();
        buf.Check("X");
        provokeY.Perform();
        buf.Check("Y");
        provokeX.Perform();
        provokeY.Perform();
        // No 'Z' because the formulas were performed separately.
        buf.Check("X, Y");

        // Perform both formulas in a group call.
        b.Group(() => {
            provokeX.Perform();
            provokeY.Perform();
        });
        buf.Check("X, Y, Z");

        // Join the formulas to preform as a group formula.
        Formula provokeZ = Formula.Join(provokeX, provokeY);
        provokeZ.Perform();
        buf.Check("X, Y, Z");
    }

    [TestMethod]
    public void TestOnChanged() {
        StringBuilder buf = new();
        Blackboard.Blackboard b = new();
        b.Perform("in int x;");
        
        IValueWatcher<int> x = b.OnChange<int>("x");
        x.OnChanged += (object? sender, ValueEventArgs<int> e) => buf.Add("X({0} → {1})", e.Previous, e.Current);

        IValueWatcher<int> y = b.OnChange<int>("y");
        y.OnChanged += (object? sender, ValueEventArgs<int> e) => buf.Add("Y({0} → {1})", e.Previous, e.Current);
        
        IValueWatcher<double> z = b.OnChange<double>("z");
        z.OnChanged += (object? sender, ValueEventArgs<double> e) => buf.Add("Z({0} → {1})", e.Previous, e.Current);

        buf.Check("");
        Formula setX1 = b.CreateFormula("x = 12;");
        Formula setX2 = b.CreateFormula("x = 34;");
        buf.Check("");
        setX1.Perform();
        buf.Check("X(0 → 12)");
        setX2.Perform();
        buf.Check("X(12 → 34)");
        setX1.Perform();
        buf.Check("X(34 → 12)");
        
        b.Perform("in int y = 9;");
        buf.Check("Y(0 → 9)");
        Formula setY1 = b.CreateFormula("y = 56;");
        Formula setY2 = b.CreateFormula("y = 78;");
        buf.Check("");
        setY1.Perform();
        buf.Check("Y(9 → 56)");
        setY2.Perform();
        buf.Check("Y(56 → 78)");
        setY1.Perform();
        buf.Check("Y(78 → 56)");
        
        b.Perform("z := x + y/100.0;");
        buf.Check("Z(0 → 12.56)");
        setX2.Perform();
        setY2.Perform();
        // 'Z' emits twice because the formulas were performed separately.
        buf.Check("X(12 → 34), Z(12.56 → 34.56), Y(56 → 78), Z(34.56 → 34.78)");
                
        // Perform both formulas in a group call.
        b.Group(() => {
            setX1.Perform();
            setY1.Perform();
        });
        buf.Check("X(34 → 12), Z(34.78 → 12.56), Y(78 → 56)");

        // Join the formulas to preform as a group formula.
        Formula set1 = Formula.Join(setX2, setY2);
        set1.Perform();
        buf.Check("X(12 → 34), Z(12.56 → 34.78), Y(56 → 78)");
    }

    [TestMethod]
    public void TestOnChangeAndOnProvokeWithNamespaces() {
        StringBuilder buf = new();
        Blackboard.Blackboard b = new();

        ITriggerWatcher x1 = b.OnProvoke("stuff.triggers.x");
        x1.OnProvoked += (object? sender, EventArgs e) => buf.Add("Stuff.Triggers.X");
        
        IValueWatcher<int> x2 = b.OnChange<int>("stuff.values.x");
        x2.OnChanged += (object? sender, ValueEventArgs<int> e) => buf.Add("Stuff.Values.X({0} → {1})", e.Previous, e.Current);
        
        buf.Check("");
        b.Perform(
            "in source = 10;",
            "namespace stuff {",
            "   namespace triggers {",
            "      x := on(source > 10);",
            "   }",
            "   namespace values {",
            "      x := clamp(source, 1, 20);",
            "   }",
            "}");
        buf.Check("Stuff.Values.X(0 → 10)");

        b.Perform("source = 30;");
        buf.Check("Stuff.Values.X(10 → 20), Stuff.Triggers.X");

        b.Perform("source = -10;");
        buf.Check("Stuff.Values.X(20 → 1)");

        b.Perform("source = 8;");
        buf.Check("Stuff.Values.X(1 → 8)");

        b.Perform("source = 12;");
        buf.Check("Stuff.Values.X(8 → 12), Stuff.Triggers.X");
    }

    [TestMethod]
    public void TestInputTriggerBasics() {
        StringBuilder buf = new();
        Blackboard.Blackboard b = new();

        // Get existing trigger
        b.Perform("in trigger x;");
        InputTrigger xTrigger = b.Provoker("x");
        ITriggerWatcher x = b.OnProvoke("x");
        x.OnProvoked += (object? sender, EventArgs e) => buf.Add("X");

        // Create new trigger
        InputTrigger yTrigger = b.Provoker("y");
        ITriggerWatcher y = b.OnProvoke("y");
        y.OnProvoked += (object? sender, EventArgs e) => buf.Add("Y"); 
        
        // Create a trigger for checking trigger resets and grouping
        b.Perform("z := x & y;");
        ITriggerWatcher z = b.OnProvoke("z");
        z.OnProvoked += (object? sender, EventArgs e) => buf.Add("Z");
        
        buf.Check("");
        xTrigger.Provoke();
        yTrigger.Provoke();
        buf.Check("X, Y");
        b.Group(() => {
            xTrigger.Provoke();
            yTrigger.Provoke();
        });
        buf.Check("X, Y, Z");
    }

    [TestMethod]
    public void TestInputValueBasics() {
        StringBuilder buf = new();
        Blackboard.Blackboard b = new();

        // Get existing value
        b.Perform("in int x;");
        InputValue<int> xValue = b.ValueInput<int>("x");
        IValueWatcher<int> x = b.OnChange<int>("x");
        x.OnChanged += (object? sender, ValueEventArgs<int> e) => buf.Add("X({0} → {1})", e.Previous, e.Current);

        // Create new value
        InputValue<int> yValue = b.ValueInput<int>("y");
        IValueWatcher<int> y = b.OnChange<int>("y");
        y.OnChanged += (object? sender, ValueEventArgs<int> e) => buf.Add("Y({0} → {1})", e.Previous, e.Current);
        
        // Create a value for checking grouping
        b.Perform("z := x + y/100.0;");
        IValueWatcher<double> z = b.OnChange<double>("z");
        z.OnChanged += (object? sender, ValueEventArgs<double> e) => buf.Add("Z({0} → {1})", e.Previous, e.Current);

        buf.Check("");
        xValue.SetValue(34);
        yValue.SetValue(78);
        // 'Z' emits twice because the formulas were performed separately.
        buf.Check("X(0 → 34), Z(0 → 34), Y(0 → 78), Z(34 → 34.78)");
                
        // Perform both formulas in a group call.
        b.Group(() => {
            xValue.SetValue(12);
            yValue.SetValue(56);
        });
        buf.Check("X(34 → 12), Z(34.78 → 12.56), Y(78 → 56)");
    }

    [TestMethod]
    public void TestInputValueTypes() {
        StringBuilder buf = new();
        Blackboard.Blackboard b = new();
        
        InputValue<T> create<T>() {
            string name = nameof(T);
            InputValue<T> input = b.ValueInput<T>("$"+name);
            b.OnChange<T>("$"+name).OnChanged +=
                (object? sender, ValueEventArgs<T> e) =>
                    buf.Add("{0}({1} → {2})", name, e.Previous, e.Current);
            return input;
        }

        InputValue<bool>   inBool   = create<bool>();
        InputValue<double> inDouble = create<double>();
        InputValue<float>  inFloat  = create<float>();
        InputValue<int>    inInt    = create<int>();
        InputValue<object> inObject = create<object>();
        InputValue<string> inString = create<string>();
        InputValue<uint>   inUint   = create<uint>();

        // TODO: Finish
    }
}
