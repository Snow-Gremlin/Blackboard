using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Nodes.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blackboard.Core.Nodes.Functions;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Types {

        static private string toStr(INode node) => node is null ? "null" : node.ToString();

        static private void checkTypeOf(INode node, Type exp) {
            Assert.AreEqual(exp, Type.TypeOf(node));
        }

        static private void checkCasts(Type t, INode node, string expMatch, string expImplicit, string expExplicit) {
            string resultMatch    = t.Match(Type.TypeOf(node)).ToString();
            string resultImplicit = toStr(t.Implicit(node));
            string resultExplicit = toStr(t.Explicit(node));
            if (resultMatch != expMatch || resultImplicit != expImplicit || resultExplicit != expExplicit) {
                System.Console.WriteLine(resultMatch    + " =[" + (resultMatch    == expMatch    ? "X" : " ") + "]= " + expMatch);
                System.Console.WriteLine(resultImplicit + " =[" + (resultImplicit == expImplicit ? "X" : " ") + "]= " + expImplicit);
                System.Console.WriteLine(resultExplicit + " =[" + (resultExplicit == expExplicit ? "X" : " ") + "]= " + expExplicit);
                Assert.Fail();
            }
        }

        static private void checkImplicit(Type t, INode node, int steps, string expImplicit) =>
            checkCasts(t, node, "Cast("+steps+")", expImplicit, "null");

        static private void checkInherit(Type t, INode node, int steps) =>
            checkCasts(t, node, "Inherit("+steps+")", node.ToString(), node.ToString());

        static private void checkNoCast(Type t, INode node) =>
            checkCasts(t, node, "None", "null", "null");

        static private void checkExplicit(Type t, INode node, string expExplicit) =>
            checkCasts(t, node, "None", "null", expExplicit);

        [TestMethod]
        public void TestTypeOf() {
            checkTypeOf(new InputTrigger(),       Type.Trigger);
            checkTypeOf(new InputValue<Bool>(),   Type.Bool);
            checkTypeOf(new InputValue<Int>(),    Type.Int);
            checkTypeOf(new InputValue<Double>(), Type.Double);
            checkTypeOf(new InputValue<String>(), Type.String);
            checkTypeOf(new FuncGroup(),          Type.FuncGroup);
            checkTypeOf(Sum<Int>.Factory(),       Type.FuncDef);
            checkTypeOf(new Namespace(),          Type.Namespace);
            checkTypeOf(new Counter<Int>(),       Type.CounterInt);
            checkTypeOf(new Counter<Double>(),    Type.CounterDouble);
            checkTypeOf(new Toggler(),            Type.Toggler);
            checkTypeOf(new Latch<Bool>(),        Type.LatchBool);
            checkTypeOf(new Latch<Int>(),         Type.LatchInt);
            checkTypeOf(new Latch<Double>(),      Type.LatchDouble);
            checkTypeOf(new Latch<String>(),      Type.LatchString);
        }

        [TestMethod]
        public void TestCastBoolInput() {
            InputValue<Bool> node = new();
            checkInherit (Type.Node,          node, 1);
            checkImplicit(Type.Trigger,       node, 0, "BoolAsTrigger(Input<bool>)");
            checkInherit (Type.Bool,          node, 0);
            checkNoCast  (Type.Int,           node);
            checkNoCast  (Type.Double,        node);
            checkImplicit(Type.String,        node, 0, "Implicit<string>(Input<bool>)");
            checkNoCast  (Type.FuncGroup,     node);
            checkNoCast  (Type.FuncDef,       node);
            checkNoCast  (Type.Namespace,     node);
            checkNoCast  (Type.CounterInt,    node);
            checkNoCast  (Type.CounterDouble, node);
            checkNoCast  (Type.Toggler,       node);
            checkNoCast  (Type.LatchBool,     node);
            checkNoCast  (Type.LatchInt,      node);
            checkNoCast  (Type.LatchDouble,   node);
            checkNoCast  (Type.LatchString,   node);
        }
        
        [TestMethod]
        public void TestCastIntLatch() {
            Latch<Int> node = new();
            checkInherit (Type.Node,          node, 2);
            checkNoCast  (Type.Trigger,       node);
            checkNoCast  (Type.Bool,          node);
            checkInherit (Type.Int,           node, 1);
            checkImplicit(Type.Double,        node, 1, "Implicit<double>(Latch<int>(null, null))");
            checkImplicit(Type.String,        node, 1, "Implicit<string>(Latch<int>(null, null))");
            checkNoCast  (Type.FuncGroup,     node);
            checkNoCast  (Type.FuncDef,       node);
            checkNoCast  (Type.Namespace,     node);
            checkNoCast  (Type.CounterInt,    node);
            checkNoCast  (Type.CounterDouble, node);
            checkNoCast  (Type.Toggler,       node);
            checkNoCast  (Type.LatchBool,     node);
            checkInherit (Type.LatchInt,      node, 0);
            checkNoCast  (Type.LatchDouble,   node);
            checkNoCast  (Type.LatchString,   node);
        }
        
        [TestMethod]
        public void TestCastDoubleCounter() {
            Counter<Double> node = new();
            checkInherit (Type.Node,          node, 2);
            checkNoCast  (Type.Trigger,       node);
            checkNoCast  (Type.Bool,          node);
            checkExplicit(Type.Int,           node, "Explicit<int>(Counter<double>(null, null, null, null, null))");
            checkInherit (Type.Double,        node, 1);
            checkImplicit(Type.String,        node, 1, "Implicit<string>(Counter<double>(null, null, null, null, null))");
            checkNoCast  (Type.FuncGroup,     node);
            checkNoCast  (Type.FuncDef,       node);
            checkNoCast  (Type.Namespace,     node);
            checkNoCast  (Type.CounterInt,    node);
            checkInherit (Type.CounterDouble, node, 0);
            checkNoCast  (Type.Toggler,       node);
            checkNoCast  (Type.LatchBool,     node);
            checkNoCast  (Type.LatchInt,      node);
            checkNoCast  (Type.LatchDouble,   node);
            checkNoCast  (Type.LatchString,   node);
        }
    }
}
