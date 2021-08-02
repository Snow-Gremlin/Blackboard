using Blackboard.Core;
using Blackboard.Core.Functions;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Functions {

        static private string toStr(INode node) => node is null ? "null" : node.ToString();

        [TestMethod]
        public void TestFunctionsOr() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find("operators", "or") as FuncGroup;

            InputTrigger       tNode = new();
            InputValue<Bool>   bNode = new();
            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();

            Assert.AreEqual("Any(Input<trigger>, Input<trigger>)",             toStr(group.Build(tNode, tNode)));
            Assert.AreEqual("Or(Input<bool>, Input<bool>)",                    toStr(group.Build(bNode, bNode)));
            Assert.AreEqual("Or(Input<bool>, Input<bool>, Input<bool>)",       toStr(group.Build(bNode, bNode, bNode)));
            Assert.AreEqual("Input<bool>",                                     toStr(group.Build(bNode)));
            Assert.AreEqual("null",                                            toStr(group.Build()));
            Assert.AreEqual("BitwiseOr(Input<int>, Input<int>)",               toStr(group.Build(iNode, iNode)));
            Assert.AreEqual("null",                                            toStr(group.Build(dNode, dNode)));
            Assert.AreEqual("null",                                            toStr(group.Build(iNode, bNode)));
            Assert.AreEqual("null",                                            toStr(group.Build(bNode, iNode)));
            Assert.AreEqual("Any(Input<trigger>, BoolAsTrigger(Input<bool>))", toStr(group.Build(tNode, bNode)));
            Assert.AreEqual("Any(BoolAsTrigger(Input<bool>), Input<trigger>)", toStr(group.Build(bNode, tNode)));
        }

        [TestMethod]
        public void TestFunctionsRound() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find("round") as FuncGroup;

            InputValue<Bool>   bNode = new();
            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();

            Assert.AreEqual("null",                                            toStr(group.Build(bNode, bNode)));
            Assert.AreEqual("Round(Implicit<double>(Input<int>), Input<int>)", toStr(group.Build(iNode, iNode)));
            Assert.AreEqual("Round(Input<double>, Input<int>)",                toStr(group.Build(dNode, iNode)));
            Assert.AreEqual("null",                                            toStr(group.Build(iNode, dNode)));
            Assert.AreEqual("null",                                            toStr(group.Build(dNode, dNode)));
            Assert.AreEqual("Round(Implicit<double>(Input<int>))",             toStr(group.Build(iNode)));
            Assert.AreEqual("Round(Input<double>)",                            toStr(group.Build(dNode)));
            Assert.AreEqual("null",                                            toStr(group.Build(dNode, dNode, dNode)));
        }

        [TestMethod]
        public void TestFunctionsSum() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find("operators", "sum") as FuncGroup;

            InputValue<Bool>   bNode = new();
            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();
            InputValue<String> sNode = new();

            Assert.AreEqual("null",                                                toStr(group.Build(bNode, bNode)));
            Assert.AreEqual("null",                                                toStr(group.Build(bNode, iNode)));
            Assert.AreEqual("Sum(Input<int>, Input<int>)",                         toStr(group.Build(iNode, iNode)));
            Assert.AreEqual("Sum(Input<double>, Implicit<double>(Input<int>))",    toStr(group.Build(dNode, iNode)));
            Assert.AreEqual("Sum(Implicit<double>(Input<int>), Input<double>)",    toStr(group.Build(iNode, dNode)));
            Assert.AreEqual("Sum(Input<double>, Input<double>)",                   toStr(group.Build(dNode, dNode)));
            Assert.AreEqual("Sum(Input<string>, Input<string>)",                   toStr(group.Build(sNode, sNode)));
            Assert.AreEqual("Sum(Implicit<string>(Input<bool>), Input<string>)",   toStr(group.Build(bNode, sNode)));
            Assert.AreEqual("Sum(Implicit<string>(Input<int>), Input<string>)",    toStr(group.Build(iNode, sNode)));
            Assert.AreEqual("Sum(Implicit<string>(Input<double>), Input<string>)", toStr(group.Build(dNode, sNode)));
        }

        [TestMethod]
        public void TestFunctionsAtan() {
            Driver driver = new();
            FuncGroup group = driver.Global.Find("atan") as FuncGroup;

            InputValue<Int>    iNode = new();
            InputValue<Double> dNode = new();

            Assert.AreEqual("null",                                                              toStr(group.Build()));
            Assert.AreEqual("Atan(Implicit<double>(Input<int>))",                                toStr(group.Build(iNode)));
            Assert.AreEqual("Atan(Input<double>)",                                               toStr(group.Build(dNode)));
            Assert.AreEqual("Atan2(Implicit<double>(Input<int>), Implicit<double>(Input<int>))", toStr(group.Build(iNode, iNode)));
            Assert.AreEqual("Atan2(Implicit<double>(Input<int>), Input<double>)",                toStr(group.Build(iNode, dNode)));
            Assert.AreEqual("Atan2(Input<double>, Implicit<double>(Input<int>))",                toStr(group.Build(dNode, iNode)));
            Assert.AreEqual("Atan2(Input<double>, Input<double>)",                               toStr(group.Build(dNode, dNode)));
            Assert.AreEqual("null",                                                              toStr(group.Build(iNode, iNode, iNode)));
            Assert.AreEqual("null",                                                              toStr(group.Build(dNode, dNode, dNode)));
        }
    }
}
