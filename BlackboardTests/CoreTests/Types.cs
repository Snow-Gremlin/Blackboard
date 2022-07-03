using Blackboard.Core.Data.Caps;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Types {

        [TestMethod]
        public void CheckAllFunctionsAreTested() {
            TestTools.SetEntriesMatch(
                Type.AllTypes.Select(t => t.Name),
                TestTools.TestTags(this.GetType()),
                "Tests do not match the existing types");
        }

        // TODO: Add similar mechanism as in Operation and Functions to keep track of which types have been tested or not.
        // TODO: Add in tests for Maker methods for each type.
        // TODO: Test Object type

        [TestMethod]
        public void TestTypeOf() {
            new InputTrigger().      CheckTypeOf(Type.Trigger);
            new InputValue<Object>().CheckTypeOf(Type.Object);
            new InputValue<Bool>().  CheckTypeOf(Type.Bool);
            new InputValue<Int>().   CheckTypeOf(Type.Int);
            new InputValue<Double>().CheckTypeOf(Type.Double);
            new InputValue<String>().CheckTypeOf(Type.String);
            new FuncGroup().         CheckTypeOf(Type.FuncGroup);
            Sum<Int>.Factory().      CheckTypeOf(Type.FuncDef);
            new Namespace().         CheckTypeOf(Type.Namespace);
            new Counter<Int>().      CheckTypeOf(Type.CounterInt);
            new Counter<Double>().   CheckTypeOf(Type.CounterDouble);
            new Toggler().           CheckTypeOf(Type.Toggler);
            new Latch<Object>().     CheckTypeOf(Type.LatchObject);
            new Latch<Bool>().       CheckTypeOf(Type.LatchBool);
            new Latch<Int>().        CheckTypeOf(Type.LatchInt);
            new Latch<Double>().     CheckTypeOf(Type.LatchDouble);
            new Latch<String>().     CheckTypeOf(Type.LatchString);
        }

        [TestMethod]
        public void TestCastBoolInput() {
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
        public void TestCastIntLatch() {
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
        public void TestCastDoubleCounter() {
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
    }
}
