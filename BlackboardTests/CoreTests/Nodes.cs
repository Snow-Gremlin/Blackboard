using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using S = System;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Nodes {

        static private void checkString(Node node, string exp) =>
            Assert.AreEqual(exp, node.ToString());

        static private void checkDepth(Node node, int exp) =>
            Assert.AreEqual(exp, node.Depth);

        static private void checkParents(Node node, string exp) =>
            Assert.AreEqual(exp, string.Join(", ", node.Parents));

        static private void checkValue(IValue<Bool> node, bool exp) =>
            Assert.AreEqual(exp, node.Value.Value);

        static private void checkLog(StringWriter buf, params string[] lines) =>
            Assert.AreEqual(string.Join(S.Environment.NewLine, lines), buf.ToString().Trim());

        [TestMethod]
        public void TestAddNodes() {
            InputValue<Bool> input1 = new();
            InputValue<Bool> input2 = new();
            InputValue<Bool> input3 = new();
            And and12  = new(input1, input2);
            Or  or123  = new(and12, input3);
            Not not123 = new(or123);

            checkString(and12,  "And(Input<bool>, Input<bool>)");
            checkString(not123, "Not(Or(And(Input<bool>, Input<bool>), Input<bool>))");

            checkParents(input1, "");
            checkParents(input2, "");
            checkParents(input3, "");
            checkParents(and12,  "Input<bool>, Input<bool>");
            checkParents(or123,  "And(Input<bool>, Input<bool>), Input<bool>");
            checkParents(not123, "Or(And(Input<bool>, Input<bool>), Input<bool>)");

            checkDepth(input1, 0);
            checkDepth(input2, 0);
            checkDepth(input3, 0);
            checkDepth(and12,  1);
            checkDepth(or123,  2);
            checkDepth(not123, 3);
        }

        [TestMethod]
        public void TestEvaluateNodes() {
            InputValue<Bool> input1 = new();
            InputValue<Bool> input2 = new();
            InputValue<Bool> input3 = new();
            And and12  = new(input1, input2);
            Or  or123  = new(and12,  input3);
            Not not123 = new(or123);
            checkValue(input1, false);
            checkValue(input2, false);
            checkValue(input3, false);
            checkValue(and12,  false);
            checkValue(or123,  false);
            checkValue(not123, true);

            Driver drv = new(new StringWriter());
            drv.Global["One"]   = input1;
            drv.Global["Two"]   = input2;
            drv.Global["Three"] = input3;

            drv.SetBool(true, "One");
            drv.SetBool(true, "Three");
            drv.Evalate();
            checkLog(drv.Log as StringWriter,
                "Eval(0): Input<bool>",
                "Eval(0): Input<bool>",
                "Eval(1): And(Input<bool>, Input<bool>)",
                "Eval(2): Or(And(Input<bool>, Input<bool>), Input<bool>)",
                "Eval(3): Not(Or(And(Input<bool>, Input<bool>), Input<bool>))");
            checkValue(input1, true);
            checkValue(input2, false);
            checkValue(input3, true);
            checkValue(and12,  false);
            checkValue(or123,  true);
            checkValue(not123, false);

            drv.Log = new StringWriter();
            drv.SetBool(false, "Three");
            drv.Evalate();
            checkLog(drv.Log as StringWriter,
                "Eval(0): Input<bool>",
                "Eval(2): Or(And(Input<bool>, Input<bool>), Input<bool>)",
                "Eval(3): Not(Or(And(Input<bool>, Input<bool>), Input<bool>))");
            checkValue(input1, true);
            checkValue(input2, false);
            checkValue(input3, false);
            checkValue(and12,  false);
            checkValue(or123,  false);
            checkValue(not123, true);

            drv.Log = new StringWriter();
            drv.SetBool(true, "Two");
            drv.Evalate();
            checkLog(drv.Log as StringWriter,
                "Eval(0): Input<bool>",
                "Eval(1): And(Input<bool>, Input<bool>)",
                "Eval(2): Or(And(Input<bool>, Input<bool>), Input<bool>)",
                "Eval(3): Not(Or(And(Input<bool>, Input<bool>), Input<bool>))");
            checkValue(input1, true);
            checkValue(input2, true);
            checkValue(input3, false);
            checkValue(and12,  true);
            checkValue(or123,  true);
            checkValue(not123, false);
        }

        [TestMethod]
        public void TestTriggerNodes() {
            Driver drv = new(S.Console.Out);
            InputTrigger inputA = new();
            InputTrigger inputB = new();
            InputTrigger inputC = new();
            drv.Global["TrigA"] = inputA;
            drv.Global["TrigB"] = inputB;
            drv.Global["TrigC"] = inputC;

            Counter<Int> counter = new(
                new Any(inputA, inputB, inputC), null,
                new All(inputA, inputB, inputC));
            drv.Global["Count"] = counter;

            OnTrue over3 = new(new GreaterThanOrEqual<Int>(counter, Literal.Int(3)));
            drv.Global["High"] = over3;

            Toggler toggle = new(over3);
            drv.Global["Toggle"] = toggle;

            bool high;
            OutputTrigger outTrig = new(over3);
            outTrig.OnProvoked += (object sender, S.EventArgs e) => high = true;

            void check(bool triggerA, bool triggerB, bool triggerC,
                int expCount, bool expHigh, bool expToggle) {
                if (triggerA) drv.Provoke("TrigA");
                if (triggerB) drv.Provoke("TrigB");
                if (triggerC) drv.Provoke("TrigC");
                high = false;
                drv.Evalate();

                int  count  = drv.GetValue<Int>("Count").Value;
                bool toggle = drv.GetValue<Bool>("Toggle").Value;

                Assert.AreEqual(expCount,  count);
                Assert.AreEqual(expHigh,   high);
                Assert.AreEqual(expToggle, toggle);
            }

            check(true,  false, false, 1, false, false);
            check(false, true,  false, 2, false, false);
            check(false, false, true,  3, true,  true);
            check(true,  true,  false, 4, false, true);
            check(true,  true,  true,  0, false, true);
            check(true,  false, true,  1, false, true);
            check(false, true,  true,  2, false, true);
            check(false, false, true,  3, true,  false);
        }
    }
}
