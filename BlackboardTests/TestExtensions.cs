using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using S = System;

namespace BlackboardTests {

    /// <summary>These are extensions to Blackboard objects for testing.</summary>
    static class TestExtensions {

        /// <summary>Checks the string of a node.</summary>
        /// <param name="node">The node to check the sting of.</param>
        /// <param name="exp">This is the expected string name.</param>
        static public void CheckString(this INode node, string exp) =>
            Assert.AreEqual(exp, node.ToString());

        /// <summary>Checks the depth of the node.</summary>
        /// <param name="node">The node to check the depth of.</param>
        /// <param name="exp">This is the expected depth.</param>
        static public void CheckDepth(this INode node, int exp) =>
            Assert.AreEqual(exp, node.Depth);

        /// <summary>Checks the list of parents of a node.</summary>
        /// <param name="node">The node to check the parents of.</param>
        /// <param name="exp">The comma seperated list of parent strings.</param>
        static public void CheckParents(this INode node, string exp) =>
            Assert.AreEqual(exp, string.Join(", ", node.Parents));

        /// <summary>Checks the boolean value of this node.</summary>
        /// <param name="node">This is the boolean node to check the type of.</param>
        /// <param name="exp">The expected boolean value.</param>
        static public void CheckValue(this INode node, bool exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<Bool>));
            Assert.AreEqual(exp, (node as IValue<Bool>).Value.Value);
        }

        /// <summary>Checks the integer value of this node.</summary>
        /// <param name="node">This is the integer node to check the type of.</param>
        /// <param name="exp">The expected integer value.</param>
        static public void CheckValue(this INode node, int exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<Int>));
            Assert.AreEqual(exp, (node as IValue<Int>).Value.Value);
        }

        /// <summary>Checks the double value of this node.</summary>
        /// <param name="node">This is the double node to check the type of.</param>
        /// <param name="exp">The expected double value.</param>
        static public void CheckValue(this INode node, double exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<Double>));
            Assert.AreEqual(exp, (node as IValue<Double>).Value.Value);
        }

        /// <summary>Checks the string value of this node.</summary>
        /// <param name="node">This is the string node to check the type of.</param>
        /// <param name="exp">The expected string value.</param>
        static public void CheckValue(this INode node, string exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<String>));
            Assert.AreEqual(exp, (node as IValue<String>).Value.Value);
        }

        /// <summary>Gets the message for the CheckValues assertions.</summary>
        /// <param name="type">The type of the value being checked.</param>
        /// <param name="names">The name of the variable to look up.</param>
        /// <returns>The message to show in the assertion.</returns>
        static private string checkValueMsg(string type, params string[] names) =>
            "Checking the " + type + " value of \""+string.Join(".", names)+"\".";

        /// <summary>Checks the boolean value of this node.</summary>
        /// <param name="driver">This is the driver to check the type of.</param>
        /// <param name="exp">The expected boolean value.</param>
        static public void CheckValue(this Driver driver, bool exp, params string[] names) =>
            Assert.AreEqual(exp, driver.GetBool(names), checkValueMsg("bool", names));

        /// <summary>Checks the integer value of this node.</summary>
        /// <param name="driver">This is the driver to check the type of.</param>
        /// <param name="exp">The expected integer value.</param>
        static public void CheckValue(this Driver driver, int exp, params string[] names) =>
            Assert.AreEqual(exp, driver.GetInt(names), checkValueMsg("int", names));

        /// <summary>Checks the double value of this node.</summary>
        /// <param name="driver">This is the driver to check the type of.</param>
        /// <param name="exp">The expected double value.</param>
        static public void CheckValue(this Driver driver, double exp, params string[] names) =>
            Assert.AreEqual(exp, driver.GetDouble(names), checkValueMsg("double", names));

        /// <summary>Checks the string value of this node.</summary>
        /// <param name="driver">This is the driver to check the type of.</param>
        /// <param name="exp">The expected string value.</param>
        static public void CheckValue(this Driver driver, string exp, params string[] names) =>
            Assert.AreEqual(exp, driver.GetString(names), checkValueMsg("string", names));

        /// <summary>Runs the driver evaluation and checks that evaluation performed as expected.</summary>
        /// <param name="driver">The driver to evaluate.</param>
        /// <param name="lines">The expected evaluation log output.</param>
        static public void CheckEvaluate(this Driver driver, params string[] lines) {
            EvalLogger logger = new();
            driver.Evaluate(logger);
            string exp = string.Join(S.Environment.NewLine, lines);
            Assert.AreEqual(exp, logger.ToString().Trim());
        }
    }
}
