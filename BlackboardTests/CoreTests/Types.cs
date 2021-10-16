using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Nodes.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Types {

        static private string toStr(INode node) => node is null ? "null" : node.ToString();

        static private void checkTypeOf(INode node, Type exp) {
            Assert.AreEqual(exp, Type.TypeOf(node));
        }

        static private void checkCast(Type t, INode node, string expMatch, string expImplicit, string expExplicit) {
            Assert.AreEqual(expMatch, t.Match(Type.TypeOf(node)).ToString());
            Assert.AreEqual(expImplicit, toStr(t.Implicit(node)));
            Assert.AreEqual(expExplicit, toStr(t.Explicit(node)));
        }

        [TestMethod]
        public void TestTypeOf() {
            checkTypeOf(new InputTrigger(),       Type.Trigger);
            checkTypeOf(new InputValue<Bool>(),   Type.Bool);
            checkTypeOf(new InputValue<Int>(),    Type.Int);
            checkTypeOf(new InputValue<Double>(), Type.Double);
            checkTypeOf(new InputValue<String>(), Type.String);
            checkTypeOf(new Counter<Int>(),       Type.CounterInt);
            checkTypeOf(new Counter<Double>(),    Type.CounterDouble);
            checkTypeOf(new Toggler(),            Type.Toggler);
        }

        [TestMethod]
        public void TestCastBoolInput() {
            InputValue<Bool> node = new();
            checkCast(Type.Node,          node, "Inherit(1)", "Input<bool>", "Input<bool>");
            checkCast(Type.Trigger,       node, "Cast(0)", "BoolAsTrigger(Input<bool>)", "null");
            checkCast(Type.Bool,          node, "Inherit(0)", "Input<bool>", "Input<bool>");
            checkCast(Type.Int,           node, "None", "null", "null");
            checkCast(Type.Double,        node, "None", "null", "null");
            checkCast(Type.String,        node, "Cast(0)", "Implicit<string>(Input<bool>)", "null");
            checkCast(Type.CounterInt,    node, "None", "null", "null");
            checkCast(Type.CounterDouble, node, "None", "null", "null");
            checkCast(Type.Toggler,       node, "None", "null", "null");
        }
        
        [TestMethod]
        public void TestCastIntLatch() {
            Latch<Int> node = new();
            checkCast(Type.Node,          node, "Inherit(1)", "Latch<int>(null, null)", "Latch<int>(null, null)");
            checkCast(Type.Trigger,       node, "None", "null", "null");
            checkCast(Type.Bool,          node, "None", "null", "null");
            checkCast(Type.Int,           node, "Inherit(0)", "Latch<int>(null, null)", "Latch<int>(null, null)");
            checkCast(Type.Double,        node, "Cast(1)", "Implicit<double>(Latch<int>(null, null))", "null");
            checkCast(Type.String,        node, "Cast(1)", "Implicit<string>(Latch<int>(null, null))", "null");
            checkCast(Type.CounterInt,    node, "None", "null", "null");
            checkCast(Type.CounterDouble, node, "None", "null", "null");
            checkCast(Type.Toggler,       node, "None", "null", "null");
        }
        
        [TestMethod]
        public void TestCastDoubleCounter() {
            Counter<Double> node = new();
            checkCast(Type.Node,          node, "Inherit(2)", "Counter<double>(null, null, null, null, null)", "Counter<double>(null, null, null, null, null)");
            checkCast(Type.Trigger,       node, "None", "null", "null");
            checkCast(Type.Bool,          node, "None", "null", "null");
            checkCast(Type.Int,           node, "None", "null", "Explicit<int>(Counter<double>(null, null, null, null, null))");
            checkCast(Type.Double,        node, "Inherit(1)", "Counter<double>(null, null, null, null, null)", "Counter<double>(null, null, null, null, null)");
            checkCast(Type.String,        node, "Cast(1)", "Implicit<string>(Counter<double>(null, null, null, null, null))", "null");
            checkCast(Type.CounterInt,    node, "None", "null", "null");
            checkCast(Type.CounterDouble, node, "Inherit(0)", "Counter<double>(null, null, null, null, null)", "Counter<double>(null, null, null, null, null)");
            checkCast(Type.Toggler,       node, "None", "null", "null");
        }
    }
}
