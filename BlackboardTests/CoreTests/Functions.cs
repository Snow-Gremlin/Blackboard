using Blackboard.Core;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Nodes.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Functions {

        static private string toStr(INode node) => node is null ? "null" : node.ToString();

        [TestMethod]
        public void TestFunctionsOr() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find(Driver.OperatorNamespace, "or") as FuncGroup;

            InputTrigger       tNode = new();
            InputValue<Bool>   bNode = new();
            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();

            Assert.AreEqual("Any(Input<trigger>, Input<trigger>)",                            toStr(group.Build(tNode, tNode)));
            Assert.AreEqual("Or(Input<bool>(False), Input<bool>(False))",                     toStr(group.Build(bNode, bNode)));
            Assert.AreEqual("Or(Input<bool>(False), Input<bool>(False), Input<bool>(False))", toStr(group.Build(bNode, bNode, bNode)));
            Assert.AreEqual("Input<bool>(False)",                                             toStr(group.Build(bNode)));
            Assert.AreEqual("null",                                                           toStr(group.Build()));
            Assert.AreEqual("BitwiseOr(Input<int>(0), Input<int>(0))",                        toStr(group.Build(iNode, iNode)));
            Assert.AreEqual("null",                                                           toStr(group.Build(dNode, dNode)));
            Assert.AreEqual("null",                                                           toStr(group.Build(iNode, bNode)));
            Assert.AreEqual("null",                                                           toStr(group.Build(bNode, iNode)));
            Assert.AreEqual("Any(Input<trigger>, BoolAsTrigger(Input<bool>(False)))",         toStr(group.Build(tNode, bNode)));
            Assert.AreEqual("Any(BoolAsTrigger(Input<bool>(False)), Input<trigger>)",         toStr(group.Build(bNode, tNode)));
        }

        [TestMethod]
        public void TestFunctionsRound() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find("round") as FuncGroup;

            InputValue<Bool>   bNode = new();
            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();

            Assert.AreEqual("null",                                                  toStr(group.Build(bNode, bNode)));
            Assert.AreEqual("Round(Implicit<double>(Input<int>(0)), Input<int>(0))", toStr(group.Build(iNode, iNode)));
            Assert.AreEqual("Round(Input<double>(0), Input<int>(0))",                toStr(group.Build(dNode, iNode)));
            Assert.AreEqual("null",                                                  toStr(group.Build(iNode, dNode)));
            Assert.AreEqual("null",                                                  toStr(group.Build(dNode, dNode)));
            Assert.AreEqual("Round(Implicit<double>(Input<int>(0)))",                toStr(group.Build(iNode)));
            Assert.AreEqual("Round(Input<double>(0))",                               toStr(group.Build(dNode)));
            Assert.AreEqual("null",                                                  toStr(group.Build(dNode, dNode, dNode)));
        }

        [TestMethod]
        public void TestFunctionsSum() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find(Driver.OperatorNamespace, "sum") as FuncGroup;

            InputValue<Bool>   bNode = new();
            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();
            InputValue<String> sNode = new();

            Assert.AreEqual("null",                                                       toStr(group.Build(bNode, bNode)));
            Assert.AreEqual("null",                                                       toStr(group.Build(bNode, iNode)));
            Assert.AreEqual("Sum(Input<int>(0), Input<int>(0))",                          toStr(group.Build(iNode, iNode)));
            Assert.AreEqual("Sum(Input<double>(0), Implicit<double>(Input<int>(0)))",     toStr(group.Build(dNode, iNode)));
            Assert.AreEqual("Sum(Implicit<double>(Input<int>(0)), Input<double>(0))",     toStr(group.Build(iNode, dNode)));
            Assert.AreEqual("Sum(Input<double>(0), Input<double>(0))",                    toStr(group.Build(dNode, dNode)));
            Assert.AreEqual("Sum(Input<string>(), Input<string>())",                      toStr(group.Build(sNode, sNode)));
            Assert.AreEqual("Sum(Implicit<string>(Input<bool>(False)), Input<string>())", toStr(group.Build(bNode, sNode)));
            Assert.AreEqual("Sum(Implicit<string>(Input<int>(0)), Input<string>())",      toStr(group.Build(iNode, sNode)));
            Assert.AreEqual("Sum(Implicit<string>(Input<double>(0)), Input<string>())",   toStr(group.Build(dNode, sNode)));
        }

        [TestMethod]
        public void TestFunctionsAtan() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find("atan") as FuncGroup;

            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();

            Assert.AreEqual("null",                                                                    toStr(group.Build()));
            Assert.AreEqual("Atan(Implicit<double>(Input<int>(0)))",                                   toStr(group.Build(iNode)));
            Assert.AreEqual("Atan(Input<double>(0))",                                                  toStr(group.Build(dNode)));
            Assert.AreEqual("Atan2(Implicit<double>(Input<int>(0)), Implicit<double>(Input<int>(0)))", toStr(group.Build(iNode, iNode)));
            Assert.AreEqual("Atan2(Implicit<double>(Input<int>(0)), Input<double>(0))",                toStr(group.Build(iNode, dNode)));
            Assert.AreEqual("Atan2(Input<double>(0), Implicit<double>(Input<int>(0)))",                toStr(group.Build(dNode, iNode)));
            Assert.AreEqual("Atan2(Input<double>(0), Input<double>(0))",                               toStr(group.Build(dNode, dNode)));
            Assert.AreEqual("null",                                                                    toStr(group.Build(iNode, iNode, iNode)));
            Assert.AreEqual("null",                                                                    toStr(group.Build(dNode, dNode, dNode)));
        }
    }
}
