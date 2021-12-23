using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        static public void CheckDepth(this IEvaluable node, int exp) =>
            Assert.AreEqual(exp, node.Depth);

        /// <summary>Checks the list of parents of a node.</summary>
        /// <param name="node">The node to check the parents of.</param>
        /// <param name="exp">The comma separated list of parent strings.</param>
        static public void CheckParents(this IChild node, string exp) =>
            Assert.AreEqual(exp, node.Parents.Join(", "));

        /// <summary>Checks the boolean value of this node.</summary>
        /// <param name="node">This is the boolean node to check the value of.</param>
        /// <param name="exp">The expected boolean value.</param>
        static public void CheckValue(this INode node, bool exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<Bool>));
            Assert.AreEqual(exp, (node as IValue<Bool>).Value.Value);
        }

        /// <summary>Checks the integer value of this node.</summary>
        /// <param name="node">This is the integer node to check the value of.</param>
        /// <param name="exp">The expected integer value.</param>
        static public void CheckValue(this INode node, int exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<Int>));
            Assert.AreEqual(exp, (node as IValue<Int>).Value.Value);
        }

        /// <summary>Checks the double value of this node.</summary>
        /// <param name="node">This is the double node to check the value of.</param>
        /// <param name="exp">The expected double value.</param>
        static public void CheckValue(this INode node, double exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<Double>));
            Assert.AreEqual(exp, (node as IValue<Double>).Value.Value);
        }

        /// <summary>Checks the string value of this node.</summary>
        /// <param name="node">This is the string node to check the value of.</param>
        /// <param name="exp">The expected string value.</param>
        static public void CheckValue(this INode node, string exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<String>));
            Assert.AreEqual(exp, (node as IValue<String>).Value.Value);
        }

        /// <summary>Checks the provoked state of this node.</summary>
        /// <param name="node">This is the trigger node to check the state of.</param>
        /// <param name="exp">The expected provoked state of the node.</param>
        static public void CheckProvoked(this INode node, bool exp) {
            Assert.IsInstanceOfType(node, typeof(ITrigger));
            Assert.AreEqual(exp, (node as ITrigger).Provoked);
        }

        /// <summary>
        /// Installs this child's parents, any parent which is a child,
        /// and any parent's parent etc.
        /// </summary>
        /// <param name="node">The node to start installing from.</param>
        static public void InstallAll(this IChild node) {
            Stack<IChild> stack = new();
            stack.Push(node);
            while (stack.Count > 0) {
                node = stack.Pop();
                node.InstallAll();
                foreach (IParent parent in node.Parents) {
                    if (parent is IChild child) stack.Push(child);
                }
            }
        }

        #endregion
        #region Actions...

        /// <summary>Checks the expected string for the given action.</summary>
        /// <param name="action">The action to check the string for.</param>
        /// <param name="lines">The lines for the expected string.</param>
        static public void Check(this IAction action, params string[] lines) =>
            TestTools.NoDiff(lines, Stringifier.Shallow(action).Trim().Split("\n"));

        #endregion
        #region Slate...

        /// <summary>Gets the message for the CheckValues assertions.</summary>
        /// <param name="type">The type of the value being checked.</param>
        /// <param name="names">The name of the variable to look up.</param>
        /// <returns>The message to show in the assertion.</returns>
        static private string checkValueMsg(string type, params string[] names) =>
            "Checking the " + type + " value of \"" + names.Join(".") + "\".";

        /// <summary>Checks the boolean value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected boolean value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckValue(this Slate slate, bool exp, params string[] names) =>
            Assert.AreEqual(exp, slate.GetBool(names), checkValueMsg("bool", names));

        /// <summary>Checks the integer value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected integer value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckValue(this Slate slate, int exp, params string[] names) =>
            Assert.AreEqual(exp, slate.GetInt(names), checkValueMsg("int", names));

        /// <summary>Checks the double value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected double value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckValue(this Slate slate, double exp, params string[] names) =>
            Assert.AreEqual(exp, slate.GetDouble(names), checkValueMsg("double", names));

        /// <summary>Checks the string value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected string value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckValue(this Slate slate, string exp, params string[] names) =>
            Assert.AreEqual(exp, slate.GetString(names), checkValueMsg("string", names));

        /// <summary>Checks the provoked state of this node.</summary>
        /// <param name="slate">This is the slate to check the state with.</param>
        /// <param name="exp">The expected provoked state of the node.</param>
        /// <param name="names">The name of the variable to look up.</param>
        static public void CheckProvoked(this Slate slate, bool exp, params string[] names) =>
            Assert.AreEqual(exp, slate.Provoked(names), "Checking the provoked state of \"" + names.Join(".") + "\".");

        /// <summary>Checks the pending nodes to evaluate are as expected.</summary>
        /// <param name="slate">This is the slate to check the pending of.</param>
        /// <param name="exp">The expected names of the pending nodes.</param>
        static public void CheckPendingEval(this Slate slate, params string[] exp) =>
            TestTools.NoDiff(exp, slate.PendingEval.Strings(), "Checking the pending nodes.");

        /// <summary>Checks the pending nodes to update are as expected.</summary>
        /// <param name="slate">This is the slate to check the pending of.</param>
        /// <param name="exp">The expected names of the pending nodes.</param>
        static public void CheckPendingUpdate(this Slate slate, params string[] exp) =>
            TestTools.NoDiff(exp, slate.PendingUpdate.Strings(), "Checking the pending nodes.");

        /// <summary>Runs the slate evaluation and checks that evaluation performed as expected.</summary>
        /// <param name="slate">The slate to evaluate.</param>
        /// <param name="lines">The expected evaluation log output.</param>
        static public void CheckEvaluate(this Slate slate, params string[] lines) {
            Logger logger = new();
            logger.Stringifier.PreloadNames(slate);
            slate.PerformEvaluation(logger);
            string exp = lines.Join("\n");
            Assert.AreEqual(exp, logger.ToString().Trim());
            // TODO: REMOVE
        }

        /// <summary>Runs the slate update and checks that evaluation performed as expected.</summary>
        /// <param name="slate">The slate to evaluate.</param>
        /// <param name="lines">The expected evaluation log output.</param>
        static public void CheckUpdate(this Slate slate, params string[] lines) {
            Logger logger = new();
            logger.Stringifier.PreloadNames(slate);
            slate.PerformUpdates(logger);
            string exp = lines.Join("\n");
            Assert.AreEqual(exp, logger.ToString().Trim());
            // TODO: REMOVE
        }

        /// <summary>Checks the deep string for the given node using the names from the given slate.</summary>
        /// <param name="slate">The slate to load the names for the nodes from.</param>
        /// <param name="node">The node to get the deep string for.</param>
        /// <param name="lines">The line to compare the node's string against.</param>
        static public void CheckNodeString(this Slate slate, INode node, params string[] lines) {
            string exp = lines.Join("\n");
            Stringifier stringifier = Stringifier.Deep();
            stringifier.PreloadNames(slate);
            Assert.AreEqual(exp, stringifier.Stringify(node));
            // TODO: REMOVE
        }

        /// <summary>Checks the namespace string for the whole graph and compares against the given lines.</summary>
        /// <param name="slate">The slate to compare against.</param>
        /// <param name="lines">The expected lines of the returned string.</param>
        static public void CheckGraphString(this Slate slate, params string[] lines) {
            string exp = lines.Join("\n");
            Assert.AreEqual(exp, Stringifier.GraphString(slate));
            // TODO: REMOVE
        }

        /// <summary>Performs a parse of the given input and commits the changes if there are no errors.</summary>
        /// <param name="slate">The slate to apply the parsed formula to.</param>
        /// <param name="input">The lines of the code to read and commit.</param>
        static public IAction Read(this Slate slate, params string[] input) => new Parser(slate).Read(input);

        /// <summary>Performs a parse of the given input and commits the changes if there are no errors.</summary>
        /// <param name="slate">The slate to apply the parsed formula to.</param>
        /// <param name="input">The lines of the code to read and commit.</param>
        static public void ReadCommit(this Slate slate, params string[] input) {
            Parser parser = new(slate);
            IAction formula = parser.Read(input);
            formula.Perform(slate);
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
        /// <param name="minSecs">The minimum amount of time to keep repeating the action.</param>
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
