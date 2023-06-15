using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BlackboardTests.CoreTests;

[TestClass]
public class Types {

    [TestMethod]
    public void CheckAllTypesAreTested() =>
        TestTools.SetEntriesMatch(
            Type.AllTypes.Select(t => t.Name),
            TestTools.TestTags(this.GetType()),
            "Tests do not match the existing types");

    // TODO: Add in tests for Maker methods for each type.

    [TestMethod]
    [TestTag("bool")]
    public void TestTypes_bool() {
        new InputValue<Bool>().CheckTypeOf(Type.Bool);
        Type.Bool.
            CheckInheritors("toggler, latch-bool").
            CheckImplicits("trigger, object, string").
            CheckExplicits("");

        InputValue<Bool> node = new();
        node.CheckInherit (Type.Node, 1);
        node.CheckImplicit(Type.Trigger, 0, "BoolAsTrigger<trigger>[](Input<bool>)");
        node.CheckImplicit(Type.Object, 0, "Implicit<object>[null](Input<bool>)");
        node.CheckInherit (Type.Bool, 0);
        node.CheckNoCast  (Type.Int);
        node.CheckNoCast  (Type.Double);
        node.CheckImplicit(Type.String, 0, "Implicit<string>[](Input<bool>)");
        node.CheckNoCast  (Type.FuncGroup);
        node.CheckNoCast  (Type.FuncDef);
        node.CheckNoCast  (Type.Namespace);
        node.CheckNoCast  (Type.CounterInt);
        node.CheckNoCast  (Type.CounterDouble);
        node.CheckNoCast  (Type.Toggler);
        node.CheckNoCast  (Type.LatchBool);
        node.CheckNoCast  (Type.LatchInt);
        node.CheckNoCast  (Type.LatchDouble);
        node.CheckNoCast  (Type.LatchString);
    }

    [TestMethod]
    [TestTag("counter-double")]
    public void TestTypes_counter_double() {
        new Counter<Double>().CheckTypeOf(Type.CounterDouble);
        Type.CounterDouble.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");

        Counter<Double> node = new();
        node.CheckInherit (Type.Node, 2);
        node.CheckNoCast  (Type.Trigger);
        node.CheckImplicit(Type.Object, 1, "Implicit<object>[null](Counter<double>(null, null, null, null, null))");
        node.CheckNoCast  (Type.Bool);
        node.CheckExplicit(Type.Int, "Explicit<int>[0](Counter<double>(null, null, null, null, null))");
        node.CheckInherit (Type.Double, 1);
        node.CheckImplicit(Type.String, 1, "Implicit<string>[](Counter<double>(null, null, null, null, null))");
        node.CheckNoCast  (Type.FuncGroup);
        node.CheckNoCast  (Type.FuncDef);
        node.CheckNoCast  (Type.Namespace);
        node.CheckNoCast  (Type.CounterInt);
        node.CheckInherit (Type.CounterDouble, 0);
        node.CheckNoCast  (Type.Toggler);
        node.CheckNoCast  (Type.LatchBool);
        node.CheckNoCast  (Type.LatchInt);
        node.CheckNoCast  (Type.LatchDouble);
        node.CheckNoCast  (Type.LatchString);
    }

    [TestMethod]
    [TestTag("counter-int")]
    public void TestTypes_counter_int() {
        new Counter<Int>().CheckTypeOf(Type.CounterInt);
        Type.CounterInt.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("double")]
    public void TestTypes_double() {
        new InputValue<Double>().CheckTypeOf(Type.Double);
        Type.Double.
            CheckInheritors("counter-double, latch-double").
            CheckImplicits("object, string").
            CheckExplicits("int");
    }

    [TestMethod]
    [TestTag("function-def")]
    public void TestTypes_function_def() {
        Sum<Int>.Factory().CheckTypeOf(Type.FuncDef);
        Type.FuncDef.
            CheckInheritors("").
            CheckImplicits("function-group").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("function-group")]
    public void TestTypes_function_group() {
        new FuncGroup().CheckTypeOf(Type.FuncGroup);
        Type.FuncGroup.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("int")]
    public void TestTypes_int() {
        new InputValue<Int>().CheckTypeOf(Type.Int);
        Type.Int.
            CheckInheritors("counter-int, latch-int").
            CheckImplicits("object, double, string").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("latch-bool")]
    public void TestTypes_latch_bool() {
        new Latch<Bool>().CheckTypeOf(Type.LatchBool);
        Type.LatchBool.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("latch-double")]
    public void TestTypes_latch_double() {
        new Latch<Double>().CheckTypeOf(Type.LatchDouble);
        Type.LatchDouble.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("latch-int")]
    public void TestTypes_latch_int() {
        new Latch<Int>().CheckTypeOf(Type.LatchInt);
        Type.LatchInt.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");

        Latch<Int> node = new();
        node.CheckInherit (Type.Node, 2);
        node.CheckNoCast  (Type.Trigger);
        node.CheckImplicit(Type.Object, 1, "Implicit<object>[null](Latch<int>(null, null))");
        node.CheckNoCast  (Type.Bool);
        node.CheckInherit (Type.Int, 1);
        node.CheckImplicit(Type.Double, 1, "Implicit<double>[0](Latch<int>(null, null))");
        node.CheckImplicit(Type.String, 1, "Implicit<string>[](Latch<int>(null, null))");
        node.CheckNoCast  (Type.FuncGroup);
        node.CheckNoCast  (Type.FuncDef);
        node.CheckNoCast  (Type.Namespace);
        node.CheckNoCast  (Type.CounterInt);
        node.CheckNoCast  (Type.CounterDouble);
        node.CheckNoCast  (Type.Toggler);
        node.CheckNoCast  (Type.LatchBool);
        node.CheckInherit (Type.LatchInt, 0);
        node.CheckNoCast  (Type.LatchDouble);
        node.CheckNoCast  (Type.LatchString);
    }

    [TestMethod]
    [TestTag("latch-object")]
    public void TestTypes_latch_object() {
        new Latch<Object>().CheckTypeOf(Type.LatchObject);
        Type.LatchObject.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("latch-string")]
    public void TestTypes_latch_string() {
        new Latch<String>().CheckTypeOf(Type.LatchString);
        Type.LatchString.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("namespace")]
    public void TestTypes_namespace() {
        new Namespace().CheckTypeOf(Type.Namespace);
        Type.Namespace.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("node")]
    public void TestTypes_node() =>
        Type.Node.
            CheckInheritors("trigger, object, bool, int, double, string, namespace, function-group, function-def").
            CheckImplicits("").
            CheckExplicits("");

    [TestMethod]
    [TestTag("object")]
    public void TestTypes_object() {
        new InputValue<Object>().CheckTypeOf(Type.Object);
        Type.Object.
            CheckInheritors("latch-object").
            CheckImplicits("string").
            CheckExplicits("bool, int, double");
    }

    [TestMethod]
    [TestTag("string")]
    public void TestTypes_string() {
        new InputValue<String>().CheckTypeOf(Type.String);
        Type.String.
            CheckInheritors("latch-string").
            CheckImplicits("object").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("toggler")]
    public void TestTypes_toggler() {
        new Toggler().CheckTypeOf(Type.Toggler);
        Type.Toggler.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");
    }

    [TestMethod]
    [TestTag("trigger")]
    public void TestTypes_trigger() {
        new InputTrigger().CheckTypeOf(Type.Trigger);
        Type.Trigger.
            CheckInheritors("").
            CheckImplicits("").
            CheckExplicits("");
    }
}
