using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using S = System;

namespace BlackboardTests {

    /// <summary>These are extensions to Blackboard objects for testing.</summary>
    static class TestExtensions {
        #region Node...

        /// <summary>Checks the string of a node.</summary>
        /// <param name="node">The node to check the sting of.</param>
        /// <param name="exp">This is the expected string name.</param>
        static public void CheckString(this INode node, string exp) =>
            Assert.AreEqual(exp, Stringifier.Shallow(node));

        /// <summary>Checks the depth of the node.</summary>
        /// <param name="node">The node to check the depth of.</param>
        /// <param name="exp">This is the expected depth.</param>
        static public void CheckDepth(this IEvaluatable node, int exp) =>
            Assert.AreEqual(exp, node.Depth);

        /// <summary>Checks the list of parents of a node.</summary>
        /// <param name="node">The node to check the parents of.</param>
        /// <param name="exp">The comma seperated list of parent strings.</param>
        static public void CheckParents(this INode node, string exp) =>
            Assert.AreEqual(exp, node.Parents.Join(", "));

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

        /// <summary>Checks the provoked state of this node.</summary>
        /// <param name="node">This is the trigger node to check the type of.</param>
        /// <param name="exp">The expected provoked state of the node.</param>
        static public void CheckProvoked(this INode node, bool exp) {
            Assert.IsInstanceOfType(node, typeof(ITrigger));
            Assert.AreEqual(exp, (node as ITrigger).Provoked);
        }

        #endregion
        #region Driver...

        /// <summary>Gets the message for the CheckValues assertions.</summary>
        /// <param name="type">The type of the value being checked.</param>
        /// <param name="names">The name of the variable to look up.</param>
        /// <returns>The message to show in the assertion.</returns>
        static private string checkValueMsg(string type, params string[] names) =>
            "Checking the " + type + " value of \"" + names.Join(".") + "\".";

        /// <summary>Checks the boolean value of this node.</summary>
        /// <param name="driver">This is the driver to check the type of.</param>
        /// <param name="exp">The expected boolean value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckValue(this Driver driver, bool exp, params string[] names) =>
            Assert.AreEqual(exp, driver.GetBool(names), checkValueMsg("bool", names));

        /// <summary>Checks the integer value of this node.</summary>
        /// <param name="driver">This is the driver to check the type of.</param>
        /// <param name="exp">The expected integer value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckValue(this Driver driver, int exp, params string[] names) =>
            Assert.AreEqual(exp, driver.GetInt(names), checkValueMsg("int", names));

        /// <summary>Checks the double value of this node.</summary>
        /// <param name="driver">This is the driver to check the type of.</param>
        /// <param name="exp">The expected double value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckValue(this Driver driver, double exp, params string[] names) =>
            Assert.AreEqual(exp, driver.GetDouble(names), checkValueMsg("double", names));

        /// <summary>Checks the string value of this node.</summary>
        /// <param name="driver">This is the driver to check the type of.</param>
        /// <param name="exp">The expected string value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckValue(this Driver driver, string exp, params string[] names) =>
            Assert.AreEqual(exp, driver.GetString(names), checkValueMsg("string", names));

        /// <summary>Checks the provoked state of this node.</summary>
        /// <param name="driver">This is the driver to check the state of.</param>
        /// <param name="exp">The expected provoked state of the node.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckProvoked(this Driver driver, bool exp, params string[] names) =>
            Assert.AreEqual(exp, driver.Provoked(names), "Checking the provoked state of \"" + names.Join(".") + "\".");

        /// <summary>Runs the driver evaluation and checks that evaluation performed as expected.</summary>
        /// <param name="driver">The driver to evaluate.</param>
        /// <param name="lines">The expected evaluation log output.</param>
        static public void CheckEvaluate(this Driver driver, params string[] lines) {
            EvalLogger logger = new();
            logger.Stringifier.PreloadNames(driver);
            driver.Evaluate(logger);
            string exp = lines.Join(S.Environment.NewLine);
            Assert.AreEqual(exp, logger.ToString().Trim());
        }

        /// <summary>Checks the deep string for the given node using the names from the given driver.</summary>
        /// <param name="driver">The driver to load the names for the nodes from.</param>
        /// <param name="node">The node to get the deep string for.</param>
        /// <param name="lines">The line to compare the node's string against.</param>
        static public void CheckNodeString(this Driver driver, INode node, params string[] lines) {
            string exp = lines.Join(S.Environment.NewLine);
            Stringifier stringifier = Stringifier.Deep();
            stringifier.PreloadNames(driver);
            Assert.AreEqual(exp, stringifier.Stringify(node));
        }

        /// <summary>Checks the namespace string for the whole graph and compares against the given lines.</summary>
        /// <param name="driver">The driver to compare against.</param>
        /// <param name="lines">The expected lines of the returned string.</param>
        static public void CheckGraphString(this Driver driver, params string[] lines) {
            string exp = lines.Join(S.Environment.NewLine);
            Assert.AreEqual(exp, Stringifier.GraphString(driver));
        }

        /// <summary>Performs a parse of the given input and commits the changes if there are no errors.</summary>
        /// <param name="driver">The driver to apply the parsed formula to.</param>
        /// <param name="input">The lines of the code to read and commit.</param>
        static public void ReadCommit(this Driver driver, params string[] input) {
            Parser parser = new(driver);
            Formula formula = parser.Read(input);
            formula.Perform();
        }

        #endregion
        #region Other...

        /// <summary>This measures the amount of time it takes to run the action several times.</summary>
        /// <param name="action">The action to run several times.</param>
        /// <param name="title">
        /// The title to show when printing measurement information to the console.
        /// If null or empty then no information is printed to the console.
        /// </param>
        /// <param name="divisor">
        /// The scalar to convert a call to an op.
        /// For example if the call makes 10 calls into some operation then the divisor is 10.
        /// </param>
        /// <param name="minSecs">The minimum abount of time to keep repeating the action.</param>
        /// <returns>The average number of milliseconds per action operation.</returns>
        static public double Measure(this S.Action action, string title = null, double divisor = 1.0, double minSecs = 0.5) {
            S.TimeSpan minimum = S.TimeSpan.FromSeconds(minSecs);
            int count = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            do {
                count++;
                action();
            } while (stopwatch.Elapsed < minimum);
            stopwatch.Stop();
            S.TimeSpan elapsed = stopwatch.Elapsed;
            S.TimeSpan perCall = elapsed.Divide(count);
            S.TimeSpan perOp   = perCall.Divide(divisor);

            if (!string.IsNullOrEmpty(title)) {
                S.Console.WriteLine(title+":");
                S.Console.WriteLine("   Total Time: "+elapsed.TotalSeconds+"s");
                S.Console.WriteLine("   Call Count: "+count);
                S.Console.WriteLine("   Per Call:   "+perCall.TotalMilliseconds+"ms");
                S.Console.WriteLine("   Per Op:     "+perOp.TotalMilliseconds+"ms");
            }
            return perOp.TotalMilliseconds;
        }

        #endregion
    }
}
