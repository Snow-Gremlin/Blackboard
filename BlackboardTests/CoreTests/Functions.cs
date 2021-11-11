using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Functions {

        static private void checkNode(INode node, string exp) =>
            Assert.AreEqual(exp, Stringifier.Shallow(node));

        [TestMethod]
        public void TestFunctionsOr() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find(Driver.OperatorNamespace, "or") as FuncGroup;

            InputTrigger       tNode = new();
            InputValue<Bool>   bNode = new();
            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();

            checkNode(group.Build(tNode, tNode),        "Any<trigger>(Input<trigger>, Input<trigger>)");
            checkNode(group.Build(bNode, bNode),        "Or<bool>(Input<bool>[False], Input<bool>[False])");
            checkNode(group.Build(bNode, bNode, bNode), "Or<bool>(Input<bool>[False], Input<bool>[False], Input<bool>[False])");
            checkNode(group.Build(bNode),               "Input<bool>[False]");
            checkNode(group.Build(),                    "null");
            checkNode(group.Build(iNode, iNode),        "BitwiseOr<int>(Input<int>[0], Input<int>[0])");
            checkNode(group.Build(dNode, dNode),        "null");
            checkNode(group.Build(iNode, bNode),        "null");
            checkNode(group.Build(bNode, iNode),        "null");
            checkNode(group.Build(tNode, bNode),        "Any<trigger>(Input<trigger>, BoolAsTrigger<bool>(Input<bool>[False]))");
            checkNode(group.Build(bNode, tNode),        "Any<trigger>(BoolAsTrigger<bool>(Input<bool>[False]), Input<trigger>)");
        }

        [TestMethod]
        public void TestFunctionsRound() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find("round") as FuncGroup;

            InputValue<Bool>   bNode = new();
            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();

            checkNode(group.Build(bNode, bNode),        "null");
            checkNode(group.Build(iNode, iNode),        "Round<double>(Implicit<double>(Input<int>[0]), Input<int>[0])");
            checkNode(group.Build(dNode, iNode),        "Round<double>(Input<double>[0], Input<int>[0])");
            checkNode(group.Build(iNode, dNode),        "null");
            checkNode(group.Build(dNode, dNode),        "null");
            checkNode(group.Build(iNode),               "Round<double>(Implicit<double>(Input<int>[0]))");
            checkNode(group.Build(dNode),               "Round<double>(Input<double>[0])");
            checkNode(group.Build(dNode, dNode, dNode), "null");
        }

        [TestMethod]
        public void TestFunctionsSum() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find(Driver.OperatorNamespace, "sum") as FuncGroup;

            InputValue<Bool>   bNode = new();
            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();
            InputValue<String> sNode = new();

            checkNode(group.Build(bNode, bNode), "null");
            checkNode(group.Build(bNode, iNode), "null");
            checkNode(group.Build(iNode, iNode), "Sum<int>(Input<int>[0], Input<int>[0])");
            checkNode(group.Build(dNode, iNode), "Sum<double>(Input<double>[0], Implicit<double>(Input<int>[0]))");
            checkNode(group.Build(iNode, dNode), "Sum<double>(Implicit<double>(Input<int>[0]), Input<double>[0])");
            checkNode(group.Build(dNode, dNode), "Sum<double>(Input<double>[0], Input<double>[0])");
            checkNode(group.Build(sNode, sNode), "Sum<string>(Input<string>[], Input<string>[])");
            checkNode(group.Build(bNode, sNode), "Sum<string>(Implicit<string>(Input<bool>[False]), Input<string>[])");
            checkNode(group.Build(iNode, sNode), "Sum<string>(Implicit<string>(Input<int>[0]), Input<string>[])");
            checkNode(group.Build(dNode, sNode), "Sum<string>(Implicit<string>(Input<double>[0]), Input<string>[])");
        }

        [TestMethod]
        public void TestFunctionsAtan() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find("atan") as FuncGroup;

            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();

            checkNode(group.Build(),                    "null");
            checkNode(group.Build(iNode),               "Atan<double>(Implicit<double>(Input<int>[0]))");
            checkNode(group.Build(dNode),               "Atan<double>(Input<double>[0])");
            checkNode(group.Build(iNode, iNode),        "Atan2<double>(Implicit<double>(Input<int>[0]), Implicit<double>(Input<int>[0]))");
            checkNode(group.Build(iNode, dNode),        "Atan2<double>(Implicit<double>(Input<int>[0]), Input<double>[0])");
            checkNode(group.Build(dNode, iNode),        "Atan2<double>(Input<double>[0], Implicit<double>(Input<int>[0]))");
            checkNode(group.Build(dNode, dNode),        "Atan2<double>(Input<double>[0], Input<double>[0])");
            checkNode(group.Build(iNode, iNode, iNode), "null");
            checkNode(group.Build(dNode, dNode, dNode), "null");
        }
    }
}
