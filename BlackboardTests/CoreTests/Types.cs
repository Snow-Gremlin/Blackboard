using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Types {

        static private string toStr(INode node) => node is null ? "null" : node.ToString();

        static private void checkTypeOf(INode node, Type exp) {
            Assert.AreEqual(exp, Type.TypeOf(node));
        }

        static private void checkCast(Type t, INode node, int expMatch, string expImplicit, string expExplicit) {
            Assert.AreEqual(expMatch, t.Match(Type.TypeOf(node)));
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
            checkTypeOf(new Latch<Bool>(),        Type.LatchBool);
            checkTypeOf(new Latch<Int>(),         Type.LatchInt);
            checkTypeOf(new Latch<Double>(),      Type.LatchDouble);
            checkTypeOf(new Latch<String>(),      Type.LatchString);
        }

        [TestMethod]
        public void TestCastBool() {
            InputValue<Bool> node = new();
            checkCast(Type.Node,          node,  1, "Input<bool>", "Input<bool>");
            checkCast(Type.Trigger,       node, 10, "BoolAsTrigger(Input<bool>)", "null");
            checkCast(Type.Bool,          node,  0, "Input<bool>", "Input<bool>");
            checkCast(Type.Int,           node, -1, "null", "null");
            checkCast(Type.Double,        node, -1, "null", "null");
            checkCast(Type.String,        node, 10, "Implicit<string>(Input<bool>)", "null");
            checkCast(Type.CounterInt,    node, -1, "null", "null");
            checkCast(Type.CounterDouble, node, -1, "null", "null");
            checkCast(Type.Toggler,       node, -1, "null", "null");
            checkCast(Type.LatchBool,     node, -1, "null", "null");
            checkCast(Type.LatchInt,      node, -1, "null", "null");
            checkCast(Type.LatchDouble,   node, -1, "null", "null");
            checkCast(Type.LatchString,   node, -1, "null", "null");
        }

        [TestMethod]
        public void TestCastInt() {
            Latch<Int> node = new();
            checkCast(Type.Node,          node,  2, "Latch<int>(null, null)", "Latch<int>(null, null)");
            checkCast(Type.Trigger,       node, -1, "null", "null");
            checkCast(Type.Bool,          node, -1, "null", "null");
            checkCast(Type.Int,           node,  1, "Latch<int>(null, null)", "Latch<int>(null, null)");
            checkCast(Type.Double,        node, 11, "Implicit<double>(Latch<int>(null, null))", "null");
            checkCast(Type.String,        node, 11, "Implicit<string>(Latch<int>(null, null))", "null");
            checkCast(Type.CounterInt,    node, -1, "null", "null");
            checkCast(Type.CounterDouble, node, -1, "null", "null");
            checkCast(Type.Toggler,       node, -1, "null", "null");
            checkCast(Type.LatchBool,     node, -1, "null", "null");
            checkCast(Type.LatchInt,      node,  0, "Latch<int>(null, null)", "Latch<int>(null, null)");
            checkCast(Type.LatchDouble,   node, -1, "null", "null");
            checkCast(Type.LatchString,   node, -1, "null", "null");
        }

        [TestMethod]
        public void TestCastDouble() {
            Counter<Double> node = new();
            checkCast(Type.Node,          node,  2, "Counter<double>(null, null, null, null, null)", "Counter<double>(null, null, null, null, null)");
            checkCast(Type.Trigger,       node, -1, "null", "null");
            checkCast(Type.Bool,          node, -1, "null", "null");
            checkCast(Type.Int,           node, -1, "null", "Explicit<int>(Counter<double>(null, null, null, null, null))");
            checkCast(Type.Double,        node,  1, "Counter<double>(null, null, null, null, null)", "Counter<double>(null, null, null, null, null)");
            checkCast(Type.String,        node, 11, "Implicit<string>(Counter<double>(null, null, null, null, null))", "null");
            checkCast(Type.CounterInt,    node, -1, "null", "null");
            checkCast(Type.CounterDouble, node,  0, "Counter<double>(null, null, null, null, null)", "Counter<double>(null, null, null, null, null)");
            checkCast(Type.Toggler,       node, -1, "null", "null");
            checkCast(Type.LatchBool,     node, -1, "null", "null");
            checkCast(Type.LatchInt,      node, -1, "null", "null");
            checkCast(Type.LatchDouble,   node, -1, "null", "null");
            checkCast(Type.LatchString,   node, -1, "null", "null");
        }
    }
}
