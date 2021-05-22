using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blackboard.Core.Caps;
using Blackboard.Core;
using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.IO;
using System.Linq;
using System;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Nodes {

        static private void checkString(Node node, string exp) =>
            Assert.AreEqual(exp, node.ToString());

        static private void checkDepth(Node node, int exp) =>
            Assert.AreEqual(exp, node.Depth);

        static private void checkParents(Node node, string exp) =>
            Assert.AreEqual(exp, string.Join(", ", node.Parents));

        static private void checkValue<T>(IValue<T> node, T exp) =>
            Assert.AreEqual(exp, node.Value);

        static private void checkLog(StringWriter buf, params string[] lines) =>
            Assert.AreEqual(string.Join(System.Environment.NewLine, lines), buf.ToString().Trim());

        [TestMethod]
        public void TestAddNodes() {
            InputValue<bool> input1 = new("One");
            InputValue<bool> input2 = new("Two");
            InputValue<bool> input3 = new("Three");
            And and12 = new(input1, input2);
            Or or123 = new(and12, input3);
            Not not123 = new(or123);

            checkString(and12, "And(One, Two)");
            checkString(not123, "Not(Or(And(One, Two), Three))");

            checkParents(input1, "");
            checkParents(input2, "");
            checkParents(input3, "");
            checkParents(and12, "One, Two");
            checkParents(or123, "And(One, Two), Three");
            checkParents(not123, "Or(And(One, Two), Three)");

            checkDepth(input1, 0);
            checkDepth(input2, 0);
            checkDepth(input3, 0);
            checkDepth(and12, 1);
            checkDepth(or123, 2);
            checkDepth(not123, 3);
        }

        [TestMethod]
        public void TestEvaluateNodes() {
            InputValue<bool> input1 = new("One");
            InputValue<bool> input2 = new("Two");
            InputValue<bool> input3 = new("Three");
            And and12 = new(input1, input2);
            Or or123 = new(and12, input3);
            Not not123 = new(or123);
            checkValue(input1, false);
            checkValue(input2, false);
            checkValue(input3, false);
            checkValue(and12, false);
            checkValue(or123, false);
            checkValue(not123, true);

            Driver drv = new(new StringWriter());
            drv.Nodes.AddChildren(input1, input2, input3);

            drv.SetValue("One", true);
            drv.SetValue("Three", true);
            drv.Evalate();
            checkLog(drv.Log as StringWriter,
                "Eval(1): One",
                "Eval(1): Three",
                "Eval(2): And(One, Two)",
                "Eval(3): Or(And(One, Two), Three)",
                "Eval(4): Not(Or(And(One, Two), Three))");
            checkValue(input1, true);
            checkValue(input2, false);
            checkValue(input3, true);
            checkValue(and12, false);
            checkValue(or123, true);
            checkValue(not123, false);

            drv.Log = new StringWriter();
            drv.SetValue("Three", false);
            drv.Evalate();
            checkLog(drv.Log as StringWriter,
                "Eval(1): Three",
                "Eval(3): Or(And(One, Two), Three)",
                "Eval(4): Not(Or(And(One, Two), Three))");
            checkValue(input1, true);
            checkValue(input2, false);
            checkValue(input3, false);
            checkValue(and12, false);
            checkValue(or123, false);
            checkValue(not123, true);

            drv.Log = new StringWriter();
            drv.SetValue("Two", true);
            drv.Evalate();
            checkLog(drv.Log as StringWriter,
                "Eval(1): Two",
                "Eval(2): And(One, Two)",
                "Eval(3): Or(And(One, Two), Three)",
                "Eval(4): Not(Or(And(One, Two), Three))");
            checkValue(input1, true);
            checkValue(input2, true);
            checkValue(input3, false);
            checkValue(and12, true);
            checkValue(or123, true);
            checkValue(not123, false);
        }

        [TestMethod]
        public void TestTriggerNodes() {
            Driver drv = new(new StringWriter());
            InputTrigger inputA = new("TrigA", drv.Nodes);
            InputTrigger inputB = new("TrigB", drv.Nodes);
            InputTrigger inputC = new("TrigC", drv.Nodes);
            Counter counter = new(
                new Any(inputA, inputB, inputC), null,
                new All(inputA, inputB, inputC));
            OnTrue over3 = new(new GreaterThan<int>(counter, new Literal<int>(3)));
            OutputValue<int> output1 = new(counter, "Count", drv.Nodes);
            OutputTrigger output2 = new(over3, "High", drv.Nodes);

            Action<bool, bool, bool, int, bool> check = delegate(bool triggerA,
                bool triggerB, bool triggerC, int expCount, bool expHigh) {
                    //drv.Trigger(inputA);



            };

        }
    }
}
