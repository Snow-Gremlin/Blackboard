using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace BlackboardTests.CoreTests;

[TestClass]
public class FunctionSelection {

    private class Tester {
        private readonly Stringifier stringifier;
        private readonly FuncGroup group;
        private readonly string funcName;
        private readonly Dictionary<string, INode> nodes;

        public Tester(params string[] names) {
            Slate slate = new();
            this.stringifier = Stringifier.Shallow();
            this.stringifier.PreLoadNames(slate);
            this.stringifier.ShowFirstDataValues = false;
                
            this.group = slate.Global.Find(names) as FuncGroup;
            Assert.IsNotNull(this.group);
            this.funcName = names.Join(".");

            this.nodes = new Dictionary<string, INode>();
            this.addNode("T", new InputTrigger());
            this.addNode("O", new InputValue<Object>());
            this.addNode("B", new InputValue<Bool>());
            this.addNode("I", new InputValue<Int>());
            this.addNode("D", new InputValue<Double>());
            this.addNode("S", new InputValue<String>());
        }

        private void addNode(string name, INode node) {
            this.stringifier.SetNodeName(name, node);
            this.nodes.Add(name, node);
        }

        public void Test(string nodeNames, string exp) {
            INode[] nodes = nodeNames.SplitAndTrim(",").Select(this.nodes.GetValueOrDefault).ToArray();
            string result = this.stringifier.Stringify(this.group.Build(nodes));
            Assert.AreEqual(exp, result, "For "+this.funcName + "(" + nodeNames + ")");
        }
    }

    [TestMethod]
    public void TestFunctionSelection_Or() {
        Tester t = new(Slate.OperatorNamespace, "or");
        t.Test("T, T",    "Any<trigger>(T, T)");
        t.Test("B, B",    "Or<bool>(B, B)");
        t.Test("B, B, B", "Or<bool>(B, B, B)");
        t.Test("B",       "B: Input<bool>");
        t.Test("",        "null");
        t.Test("O, O",    "null");
        t.Test("I, I",    "BitwiseOr<int>(I, I)");
        t.Test("D, D",    "null");
        t.Test("I, B",    "null");
        t.Test("B, I",    "null");
        t.Test("T, B",    "Any<trigger>(T, BoolAsTrigger<trigger>(B))");
        t.Test("B, T",    "Any<trigger>(BoolAsTrigger<trigger>(B), T)");
    }

    [TestMethod]
    public void TestFunctionSelection_Round() {
        Tester t = new("round");
        t.Test("B, B",    "null");
        t.Test("O, O",    "null");
        t.Test("I, I",    "Round<double>(Implicit<double>(I), I)");
        t.Test("D, I",    "Round<double>(D, I)");
        t.Test("I, D",    "null");
        t.Test("D, D",    "null");
        t.Test("I",       "Round<double>(Implicit<double>(I))");
        t.Test("D",       "Round<double>(D)");
        t.Test("D, D, D", "null");
    }

    [TestMethod]
    public void TestFunctionSelection_Sum() {
        Tester t = new(Slate.OperatorNamespace, "sum");
        t.Test("B, B",       "null");
        t.Test("B, I",       "null");
        t.Test("O, O",       "null");
        t.Test("I, I",       "Sum<int>(I, I)");
        t.Test("D, I",       "Sum<double>(D, Implicit<double>(I))");
        t.Test("I, D",       "Sum<double>(Implicit<double>(I), D)");
        t.Test("D, D",       "Sum<double>(D, D)");
        t.Test("S, S",       "Sum<string>(S, S)");
        t.Test("B, S",       "Sum<string>(Implicit<string>(B), S)");
        t.Test("I, S",       "Sum<string>(Implicit<string>(I), S)");
        t.Test("D, S",       "Sum<string>(Implicit<string>(D), S)");
        t.Test("S, B",       "Sum<string>(S, Implicit<string>(B))");
        t.Test("S, I",       "Sum<string>(S, Implicit<string>(I))");
        t.Test("S, D",       "Sum<string>(S, Implicit<string>(D))");
        t.Test("I, D, S",    "Sum<string>(Implicit<string>(I), Implicit<string>(D), S)");
        t.Test("B, I, S, D", "Sum<string>(Implicit<string>(B), Implicit<string>(I), S, Implicit<string>(D))");
    }

    [TestMethod]
    public void TestFunctionSelection_Atan() {
        Tester t = new("atan");
        t.Test("",        "null");
        t.Test("O",       "null");
        t.Test("I",       "Atan<double>(Implicit<double>(I))");
        t.Test("D",       "Atan<double>(D)");
        t.Test("O, O",    "null");
        t.Test("I, I",    "Atan2<double>(Implicit<double>(I), Implicit<double>(I))");
        t.Test("I, D",    "Atan2<double>(Implicit<double>(I), D)");
        t.Test("D, I",    "Atan2<double>(D, Implicit<double>(I))");
        t.Test("D, D",    "Atan2<double>(D, D)");
        t.Test("I, I, I", "null");
        t.Test("D, D, D", "null");
    }

    [TestMethod]
    public void TestFunctionSelection_Format() {
        Tester t = new("format");
        t.Test("",        "null");
        t.Test("T",       "null");
        t.Test("O",       "Format<string>(Implicit<string>(O))");
        t.Test("B",       "Format<string>(Implicit<string>(B))");
        t.Test("I",       "Format<string>(Implicit<string>(I))");
        t.Test("D",       "Format<string>(Implicit<string>(D))");
        t.Test("S",       "Format<string>(S)");
        t.Test("I, I",    "Format<string>(Implicit<string>(I), Implicit<object>(I))");
        t.Test("S, I, D", "Format<string>(S, Implicit<object>(I), Implicit<object>(D))");
        t.Test("S, O, S", "Format<string>(S, O, Implicit<object>(S))");
        t.Test("S, B",    "Format<string>(S, Implicit<object>(B))");
        t.Test("S, T",    "null");
    }
}
