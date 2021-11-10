using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Outer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S = System;

namespace BlackboardTests.CoreTests {

    [TestClass]
    public class Nodes {

        [TestMethod]
        public void TestAddNodes() {
            InputValue<Bool> input1 = new();
            InputValue<Bool> input2 = new();
            InputValue<Bool> input3 = new();
            And and12  = new(input1, input2);
            Or  or123  = new(and12, input3);
            Not not123 = new(or123);

            and12 .CheckString("And<bool>(Input<bool>[False](), Input<bool>[False]())");
            not123.CheckString("Not(Or(And(Input<bool>(False), Input<bool>(False)), Input<bool>(False)))");

            input1.CheckParents("");
            input2.CheckParents("");
            input3.CheckParents("");
            and12 .CheckParents("Input<bool>(False), Input<bool>(False)");
            or123 .CheckParents("And(Input<bool>(False), Input<bool>(False)), Input<bool>(False)");
            not123.CheckParents("Or(And(Input<bool>(False), Input<bool>(False)), Input<bool>(False))");

            input1.CheckDepth(0);
            input2.CheckDepth(0);
            input3.CheckDepth(0);
            and12 .CheckDepth(1);
            or123 .CheckDepth(2);
            not123.CheckDepth(3);
        }

        [TestMethod]
        public void TestEvaluateNodes() {
            InputValue<Bool> input1 = new();
            InputValue<Bool> input2 = new();
            InputValue<Bool> input3 = new();
            And and12  = new(input1, input2);
            Or  or123  = new(and12,  input3);
            Not not123 = new(or123);
            input1.CheckValue(false);
            input2.CheckValue(false);
            input3.CheckValue(false);
            and12 .CheckValue(false);
            or123 .CheckValue(false);
            not123.CheckValue(true);

            Driver drv = new();
            drv.Global["One"]   = input1;
            drv.Global["Two"]   = input2;
            drv.Global["Three"] = input3;

            drv.SetBool(true, "One");
            drv.SetBool(true, "Three");
            drv.CheckEvaluate(
                "Start(Pending: 2)",
                "  Eval(0): Input<bool>(True)",
                "  Eval(0): Input<bool>(True)",
                "  Eval(1): And(Input<bool>(True), Input<bool>(False))",
                "  Eval(2): Or(And(Input<bool>(True), Input<bool>(False)), Input<bool>(True))",
                "  Eval(3): Not(Or(And(Input<bool>(True), Input<bool>(False)), Input<bool>(True)))",
                "End(Provoked: 0)");
            input1.CheckValue(true);
            input2.CheckValue(false);
            input3.CheckValue(true);
            and12 .CheckValue(false);
            or123 .CheckValue(true);
            not123.CheckValue(false);

            drv.SetBool(false, "Three");
            drv.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): Input<bool>(False)",
                "  Eval(2): Or(And(Input<bool>(True), Input<bool>(False)), Input<bool>(False))",
                "  Eval(3): Not(Or(And(Input<bool>(True), Input<bool>(False)), Input<bool>(False)))",
                "End(Provoked: 0)");
            input1.CheckValue(true);
            input2.CheckValue(false);
            input3.CheckValue(false);
            and12 .CheckValue(false);
            or123 .CheckValue(false);
            not123.CheckValue(true);

            drv.SetBool(true, "Two");
            drv.CheckEvaluate(
                "Start(Pending: 1)",
                "  Eval(0): Input<bool>(True)",
                "  Eval(1): And(Input<bool>(True), Input<bool>(True))",
                "  Eval(2): Or(And(Input<bool>(True), Input<bool>(True)), Input<bool>(False))",
                "  Eval(3): Not(Or(And(Input<bool>(True), Input<bool>(True)), Input<bool>(False)))",
                "End(Provoked: 0)");
            input1.CheckValue(true);
            input2.CheckValue(true);
            input3.CheckValue(false);
            and12 .CheckValue(true);
            or123 .CheckValue(true);
            not123.CheckValue(false);
        }

        [TestMethod]
        public void TestTriggerNodes() {
            Driver drv = new();
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
                drv.Evaluate();

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
