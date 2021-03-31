using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blackboard.Core.Caps;
using Blackboard.Core;
using Blackboard.Core.Interfaces;
using System.IO;
using System.Linq;

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
            Assert.AreEqual(string.Join(System.Environment.NewLine, lines),
                buf.ToString().Trim());

        [TestMethod]
        public void TestAddNodes() {
            InputValue<bool> input1 = new InputValue<bool>("One");
            InputValue<bool> input2 = new InputValue<bool>("Two");
            InputValue<bool> input3 = new InputValue<bool>("Three");
            And and12 = new And(input1, input2);
            Or or123 = new Or(and12, input3);
            Not not123 = new Not(or123);

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
            InputValue<bool> input1 = new InputValue<bool>("One");
            InputValue<bool> input2 = new InputValue<bool>("Two");
            InputValue<bool> input3 = new InputValue<bool>("Three");
            And and12 = new And(input1, input2);
            Or or123 = new Or(and12, input3);
            Not not123 = new Not(or123);
            checkValue(input1, false);
            checkValue(input2, false);
            checkValue(input3, false);
            checkValue(and12, false);
            checkValue(or123, false);
            checkValue(not123, true);

            StringWriter buf = new StringWriter();
            input1.SetValue(true);
            input3.SetValue(true);
            Evaluator.Eval(buf, input1, input3);
            checkLog(buf,
                "Eval(0): One",
                "Eval(0): Three",
                "Eval(1): And(One, Two)",
                "Eval(2): Or(And(One, Two), Three)",
                "Eval(3): Not(Or(And(One, Two), Three))");
            checkValue(input1, true);
            checkValue(input2, false);
            checkValue(input3, true);
            checkValue(and12, false);
            checkValue(or123, true);
            checkValue(not123, false);

            buf = new StringWriter();
            input3.SetValue(false);
            Evaluator.Eval(buf, input3);
            checkLog(buf,
                "Eval(0): Three",
                "Eval(2): Or(And(One, Two), Three)",
                "Eval(3): Not(Or(And(One, Two), Three))");
            checkValue(input1, true);
            checkValue(input2, false);
            checkValue(input3, false);
            checkValue(and12, false);
            checkValue(or123, false);
            checkValue(not123, true);

            buf = new StringWriter();
            input2.SetValue(true);
            Evaluator.Eval(buf, input2);
            checkLog(buf,
                "Eval(0): Two",
                "Eval(1): And(One, Two)",
                "Eval(2): Or(And(One, Two), Three)",
                "Eval(3): Not(Or(And(One, Two), Three))");
            checkValue(input1, true);
            checkValue(input2, true);
            checkValue(input3, false);
            checkValue(and12, true);
            checkValue(or123, true);
            checkValue(not123, false);
        }
    }
}
