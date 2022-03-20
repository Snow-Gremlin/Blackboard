using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Inspect;
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

            not123.LegitimatizeAll();
            not123.UpdateAllParents();
            not123.EvaluateAllParents();

            and12 .CheckString("And<bool>[false](Input<bool>, Input<bool>)");
            not123.CheckString("Not<bool>[true](Or<bool>(And<bool>, Input<bool>))");

            and12 .CheckParents("Input<bool>[false], Input<bool>[false]");
            or123 .CheckParents("And<bool>[false], Input<bool>[false]");
            not123.CheckParents("Or<bool>[false]");

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

            not123.LegitimatizeAll();
            not123.UpdateAllParents();
            not123.EvaluateAllParents();

            input1.CheckValue(false);
            input2.CheckValue(false);
            input3.CheckValue(false);
            and12 .CheckValue(false);
            or123 .CheckValue(false);
            not123.CheckValue(true);

            Slate drv = new(addConsts: false);
            drv.Global["One"]   = input1;
            drv.Global["Two"]   = input2;
            drv.Global["Three"] = input3;

            drv.SetBool(true, "One");
            drv.SetBool(true, "Three");
            drv.CheckEvaluate(
                "Start Eval (pending: 2)",
                "  Evaluated (changed: False, depth: 1, node: And<bool>[false](One, Two), remaining: 1)",
                "  Evaluated (changed: True, depth: 2, node: Or<bool>[true](And<bool>(One, Two), Three), remaining: 1)",
                "  Evaluated (changed: True, depth: 3, node: Not<bool>[false](Or<bool>(And<bool>, Three)), remaining: 0)",
                "End Eval (provoked: 0)");
            input1.CheckValue(true);
            input2.CheckValue(false);
            input3.CheckValue(true);
            and12 .CheckValue(false);
            or123 .CheckValue(true);
            not123.CheckValue(false);

            drv.SetBool(false, "Three");
            drv.CheckEvaluate(
                "Start Eval (pending: 1)",
                "  Evaluated (changed: True, depth: 2, node: Or<bool>[false](And<bool>(One, Two), Three), remaining: 1)",
                "  Evaluated (changed: True, depth: 3, node: Not<bool>[true](Or<bool>(And<bool>, Three)), remaining: 0)",
                "End Eval (provoked: 0)");
            input1.CheckValue(true);
            input2.CheckValue(false);
            input3.CheckValue(false);
            and12 .CheckValue(false);
            or123 .CheckValue(false);
            not123.CheckValue(true);

            drv.SetBool(true, "Two");
            drv.CheckEvaluate(
                "Start Eval (pending: 1)",
                "  Evaluated (changed: True, depth: 1, node: And<bool>[true](One, Two), remaining: 1)",
                "  Evaluated (changed: True, depth: 2, node: Or<bool>[true](And<bool>(One, Two), Three), remaining: 1)",
                "  Evaluated (changed: True, depth: 3, node: Not<bool>[false](Or<bool>(And<bool>, Three)), remaining: 0)",
                "End Eval (provoked: 0)");
            input1.CheckValue(true);
            input2.CheckValue(true);
            input3.CheckValue(false);
            and12 .CheckValue(true);
            or123 .CheckValue(true);
            not123.CheckValue(false);
        }

        [TestMethod]
        public void TestTriggerNodes() {
            Slate drv = new();
            InputTrigger inputA = new();
            InputTrigger inputB = new();
            InputTrigger inputC = new();
            drv.Global["TrigA"] = inputA;
            drv.Global["TrigB"] = inputB;
            drv.Global["TrigC"] = inputC;

            Any any = new(inputA, inputB, inputC);
            All all = new(inputA, inputB, inputC);
            Counter<Int> counter = new(any, null, all);
            drv.Global["Count"] = counter;

            OnTrue over3 = new(new GreaterThanOrEqual<Int>(counter, Literal.Int(3)));
            drv.Global["High"] = over3;

            Toggler toggle = new(over3);
            drv.Global["Toggle"] = toggle;

            bool high;
            OutputTrigger outTrig = new(over3);
            outTrig.OnProvoked += (object sender, S.EventArgs e) => high = true;

            outTrig.LegitimatizeAll();
            toggle.LegitimatizeAll();
            outTrig.UpdateAllParents();
            toggle.UpdateAllParents();

            void check(bool triggerA, bool triggerB, bool triggerC,
                int expCount, bool expHigh, bool expToggle) {
                if (triggerA) drv.Provoke("TrigA");
                if (triggerB) drv.Provoke("TrigB");
                if (triggerC) drv.Provoke("TrigC");
                high = false;

                drv.PerformEvaluation();
                drv.ResetTriggers();

                drv.CheckValue(expCount, "Count");
                Assert.AreEqual(expHigh, high, "High flag state");
                drv.CheckValue(expToggle, "Toggle");
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

        // TODO: Test for:
        // - setting children to parent, legitimize children.
        // - setting parents for sum(a, b, c, d) where a & c are legitimate and b & d are illegitimate.
    }
}
