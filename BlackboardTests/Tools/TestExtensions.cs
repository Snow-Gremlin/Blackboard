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

namespace BlackboardTests.Tools {

    /// <summary>These are extensions to Blackboard objects for testing.</summary>
    static class TestExtensions {
        #region Node...

        /// <summary>Checks the string of a node.</summary>
        /// <param name="node">The node to check the sting of.</param>
        /// <param name="exp">This is the expected string name.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public INode CheckString(this INode node, string exp) {
            Assert.AreEqual(exp, Stringifier.Shallow(node));
            return node;
        }

        /// <summary>Checks the depth of the node.</summary>
        /// <param name="node">The node to check the depth of.</param>
        /// <param name="exp">This is the expected depth.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public IEvaluable CheckDepth(this IEvaluable node, int exp) {
            Assert.AreEqual(exp, node.Depth);
            return node;
        }

        /// <summary>Checks the list of parents of a node.</summary>
        /// <param name="node">The node to check the parents of.</param>
        /// <param name="exp">The comma separated list of parent strings.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public IChild CheckParents(this IChild node, string exp) {
            Assert.AreEqual(exp, node.Parents.Join(", "));
            return node;
        }

        /// <summary>Checks the boolean value of this node.</summary>
        /// <param name="node">This is the boolean node to check the value of.</param>
        /// <param name="exp">The expected boolean value.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public INode CheckValue(this INode node, bool exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<Bool>));
            Assert.AreEqual(exp, (node as IValue<Bool>).Value.Value);
            return node;
        }

        /// <summary>Checks the integer value of this node.</summary>
        /// <param name="node">This is the integer node to check the value of.</param>
        /// <param name="exp">The expected integer value.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public INode CheckValue(this INode node, int exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<Int>));
            Assert.AreEqual(exp, (node as IValue<Int>).Value.Value);
            return node;
        }

        /// <summary>Checks the double value of this node.</summary>
        /// <param name="node">This is the double node to check the value of.</param>
        /// <param name="exp">The expected double value.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public INode CheckValue(this INode node, double exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<Double>));
            Assert.AreEqual(exp, (node as IValue<Double>).Value.Value);
            return node;
        }

        /// <summary>Checks the string value of this node.</summary>
        /// <param name="node">This is the string node to check the value of.</param>
        /// <param name="exp">The expected string value.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public INode CheckValue(this INode node, string exp) {
            Assert.IsInstanceOfType(node, typeof(IValue<String>));
            Assert.AreEqual(exp, (node as IValue<String>).Value.Value);
            return node;
        }

        /// <summary>Checks the provoked state of this node.</summary>
        /// <param name="node">This is the trigger node to check the state of.</param>
        /// <param name="exp">The expected provoked state of the node.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public INode CheckProvoked(this INode node, bool exp) {
            Assert.IsInstanceOfType(node, typeof(ITrigger));
            Assert.AreEqual(exp, (node as ITrigger).Provoked);
            return node;
        }

        /// <summary>Gets all reachable nodes by walking up the parents.</summary>
        /// <param name="node">The node to start walking up the parents.</param>
        /// <returns>
        /// The depth first trace through all the parents starting from the given node.
        /// The first node will be the given node.
        /// </returns>
        static public IEnumerable<INode> GetAllParents(this INode node) {
            Stack<INode> stack = new();
            stack.Push(node);
            while (stack.Count > 0) {
                node = stack.Pop();
                yield return node;

                if (node is IChild child)
                    child.Parents.Foreach(stack.Push);
            }
        }

        /// <summary>Gets a sorted link list of all the evaluable nodes from the given nodes.</summary>
        /// <param name="nodes">The nodes to get the sorted link list from.</param>
        /// <returns>The sorted link list of evaluable nodes.</returns>
        static public LinkedList<IEvaluable> ToEvalList(this IEnumerable<INode> nodes) {
            LinkedList<IEvaluable> list = new();
            list.SortInsertUnique(nodes.NotNull().OfType<IEvaluable>());
            return list;
        }

        /// <summary>Will assign all the children reachable via parents to the parents.</summary>
        /// <param name="node">The node to start adding to the parents from.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public INode LegitimatizeAll(this INode node) {
            node.GetAllParents().OfType<IChild>().Foreach(child => child.Legitimatize());
            return node;
        }

        /// <summary>Will perform depth update on all the nodes reachable from the parents.</summary>
        /// <param name="node">The node to start updating from and to get all parents and parent parents from.</param>
        /// <param name="logger">Optional logger to use to debug the update with.</param>
        /// <returns>Returns the given node so that method calls can be chained.</returns>
        static public INode UpdateAllParents(this INode node, Logger logger = null) {
            node.GetAllParents().ToEvalList().UpdateDepths(logger);
            return node;
        }

        /// <summary>Will perform evaluation on all the nodes reachable from the parents.</summary>
        /// <param name="node">The node to start evaluate from and to get all parents and parent parents from.</param>
        /// <param name="logger">Optional logger to use to debug the update with.</param>
        /// <returns>All the triggers which have been provoked and need to be reset.</returns>
        static public HashSet<ITrigger> EvaluateAllParents(this INode node, Logger logger = null) =>
            node.GetAllParents().ToEvalList().Evaluate(logger);

        #endregion
        #region Formula...

        /// <summary>Performs the formula and outputs the logs to the console.</summary>
        /// <param name="formula">The formula to perform.</param>
        /// <returns>The result from the perform.</returns>
        static public Result LogPerform(this Formula formula) =>
            formula.Perform(new ConsoleLogger());

        /// <summary>Performs the formula and checks that the log output as expected..</summary>
        /// <param name="formula">The formula to perform.</param>
        /// <param name="lines">The expected evaluation log output.</param>
        /// <returns>The result from the perform.</returns>
        static public Result CheckPerform(this Formula formula, params string[] lines) {
            BufferLogger logger = new();
            Result result = formula.Perform(logger.Stringify(Stringifier.Shallow().PreloadNames(formula.Slate)));
            TestTools.NoDiff(lines.Join("\n"), logger.ToString().Trim());
            return result;
        }

        /// <summary>Checks the expected string for the given formula.</summary>
        /// <param name="formula">The formula to check the string for.</param>
        /// <param name="lines">The lines for the expected string.</param>
        /// <returns>The formula that is passed in so this can be chained.</returns>
        static public Formula Check(this Formula formula, params string[] lines) {
            TestTools.NoDiff(lines, Stringifier.Deep(formula).Trim().Split("\n"));
            return formula;
        }

        /// <summary>Creates a new formula without the finish action.</summary>
        /// <param name="formula">The formula to copy without the finish action.</param>
        /// <returns>The new formula copy from the given formula but without the finish action.</returns>
        static public Formula NoFinish(this Formula formula) =>
            new(formula.Slate, formula.Actions.Where(action => action is not Finish));

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
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckValue(this Slate slate, bool exp, params string[] names) {
            Assert.AreEqual(exp, slate.GetBool(names), checkValueMsg("bool", names));
            return slate;
        }

        /// <summary>Checks the integer value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected integer value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckValue(this Slate slate, int exp, params string[] names) {
            Assert.AreEqual(exp, slate.GetInt(names), checkValueMsg("int", names));
            return slate;
        }

        /// <summary>Checks the double value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected double value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckValue(this Slate slate, double exp, params string[] names) {
            Assert.AreEqual(exp, slate.GetDouble(names), checkValueMsg("double", names));
            return slate;
        }

        /// <summary>Checks the string value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected string value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckValue(this Slate slate, string exp, params string[] names) {
            Assert.AreEqual(exp, slate.GetString(names), checkValueMsg("string", names));
            return slate;
        }

        /// <summary>Checks the object value of this node.</summary>
        /// <param name="slate">This is the slate to check the object with.</param>
        /// <param name="exp">The expected object value.</param>
        /// <param name="names">The name of the variable to look up.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckValueObject(this Slate slate, object exp, params string[] names) {
            Assert.AreEqual(exp, slate.GetValueObject(names), checkValueMsg("object", names));
            return slate;
        }

        /// <summary>Checks the provoked state of this node.</summary>
        /// <param name="slate">This is the slate to check the state with.</param>
        /// <param name="exp">The expected provoked state of the node.</param>
        /// <param name="names">The name of the variable to look up.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckProvoked(this Slate slate, bool exp, params string[] names) {
            Assert.AreEqual(exp, slate.Provoked(names), "Checking the provoked state of \"" + names.Join(".") + "\".");
            return slate;
        }

        /// <summary>Checks the pending nodes to evaluate are as expected.</summary>
        /// <param name="slate">This is the slate to check the pending of.</param>
        /// <param name="exp">The expected names of the pending nodes.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckPendingEval(this Slate slate, params string[] exp) {
            TestTools.NoDiff(exp, slate.PendingEval.Strings(), "Checking the pending nodes.");
            return slate;
        }

        /// <summary>Checks the pending nodes to update are as expected.</summary>
        /// <param name="slate">This is the slate to check the pending of.</param>
        /// <param name="exp">The expected names of the pending nodes.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckPendingUpdate(this Slate slate, params string[] exp) {
            TestTools.NoDiff(exp, slate.PendingUpdate.Strings(), "Checking the pending nodes.");
            return slate;
        }

        /// <summary>Runs the slate evaluation and checks that evaluation performed as expected.</summary>
        /// <param name="slate">The slate to evaluate.</param>
        /// <param name="lines">The expected evaluation log output.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckEvaluate(this Slate slate, params string[] lines) {
            BufferLogger logger = new(false);
            slate.PerformEvaluation(logger.Stringify(Stringifier.Shallow().PreloadNames(slate)));
            TestTools.NoDiff(lines.Join("\n"), logger.ToString().Trim());
            return slate;
        }

        /// <summary>Runs the slate update and checks that evaluation performed as expected.</summary>
        /// <param name="slate">The slate to evaluate.</param>
        /// <param name="lines">The expected evaluation log output.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckUpdate(this Slate slate, params string[] lines) {
            BufferLogger logger = new(false);
            slate.PerformUpdates(logger.Stringify(Stringifier.Shallow().PreloadNames(slate)));
            TestTools.NoDiff(lines.Join("\n"), logger.ToString().Trim());
            return slate;
        }

        /// <summary>Checks the string for the given node using the names from the given slate.</summary>
        /// <param name="slate">The slate to load the names for the nodes from.</param>
        /// <param name="stringifier">The stringifier to stringify with.</param>
        /// <param name="node">The node to get the string for.</param>
        /// <param name="lines">The line to compare the node's string against.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckNodeString(this Slate slate, Stringifier stringifier, INode node, params string[] lines) {
            stringifier.PreloadNames(slate);
            TestTools.NoDiff(lines.Join("\n"), stringifier.Stringify(node));
            return slate;
        }

        /// <summary>Checks the string for the given node using the names from the given slate.</summary>
        /// <param name="slate">The slate to load the names for the nodes from.</param>
        /// <param name="stringifier">The stringifier to stringify with.</param>
        /// <param name="nodeName">The name of the node to get the string for.</param>
        /// <param name="lines">The line to compare the node's string against.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckNodeString(this Slate slate, Stringifier stringifier, string nodeName, params string[] lines) =>
            slate.CheckNodeString(stringifier, slate.GetNode<INode>(nodeName), lines);

        /// <summary>Checks the deep string for the given node using the names from the given slate.</summary>
        /// <param name="slate">The slate to load the names for the nodes from.</param>
        /// <param name="nodeName">The name of the node to get the deep string for.</param>
        /// <param name="lines">The line to compare the node's string against.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckNodeString(this Slate slate, INode node, params string[] lines) =>
            slate.CheckNodeString(Stringifier.Deep(), node, lines);

        /// <summary>Checks the deep string for the given node using the names from the given slate.</summary>
        /// <param name="slate">The slate to load the names for the nodes from.</param>
        /// <param name="nodeName">The name of the node to get the deep string for.</param>
        /// <param name="lines">The line to compare the node's string against.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckNodeString(this Slate slate, string nodeName, params string[] lines) =>
            slate.CheckNodeString(slate.GetNode<INode>(nodeName), lines);

        /// <summary>Checks the namespace string for the whole graph and compares against the given lines.</summary>
        /// <param name="slate">The slate to compare against.</param>
        /// <param name="lines">The expected lines of the returned string.</param>
        /// <returns>Returns the slate so that method calls can be chained together.</returns>
        static public Slate CheckGraphString(this Slate slate, params string[] lines) {
            TestTools.NoDiff(lines.Join("\n"), Stringifier.GraphString(slate));
            return slate;
        }

        /// <summary>Performs a parse of the given input and returns the formula for the input.</summary>
        /// <param name="slate">The slate to apply the parsed formula to.</param>
        /// <param name="input">The lines of the code to read and get the formula for.</param>
        /// <remarks>Returns the formula for the read input.</remarks>
        static public Formula Read(this Slate slate, params string[] input) =>
            new Parser(slate).Read(input);

        /// <summary>Performs a parse of the given input and logs the parse output.</summary>
        /// <param name="slate">The slate to apply the parsed formula to.</param>
        /// <param name="input">The lines of the code to read and commit.</param>
        static public Formula LogRead(this Slate slate, params string[] input) {
            Stringifier stringifier = Stringifier.Deep().PreloadNames(slate);
            stringifier.ShowFuncs = false;
            Logger logger = new ConsoleLogger().Stringify(stringifier);
            return new Parser(slate, logger).Read(input);
        }

        /// <summary>Performs a parse of the given input and performs the formula.</summary>
        /// <param name="slate">The slate to apply the parsed formula to.</param>
        /// <param name="input">The lines of the code to read and perform.</param>
        /// <returns>The given slate so that method calls can be chained.</returns>
        static public Slate Perform(this Slate slate, params string[] input) {
            slate.Read(input).Perform();
            return slate;
        }

        /// <summary>Performs the given input and evaluates but doesn't reset the triggers.</summary>
        /// <param name="slate">The slate to apply the parsed formula to.</param>
        /// <param name="input">The lines of the code to read and perform.</param>
        /// <returns>The given slate so that method calls can be chained.</returns>
        static public Slate PerformWithoutReset(this Slate slate, string input) {
            slate.Read(input).NoFinish().Perform();
            slate.PerformEvaluation();
            return slate;
        }

        /// <summary>Performs a parse using the given comparison input and checks if the result it true.</summary>
        /// <param name="slate">The slate to perform this comparison on.</param>
        /// <param name="comparison">The comparison to perform and check if true.</param>
        /// <returns>The given slate so that method calls can be chained.</returns>
        static public Slate IsTrue(this Slate slate, string comparison) {
            const string returnName = "comparisonResult";
            Result result = slate.Read("get bool " + returnName + " = " + comparison + ";").Perform();
            Assert.IsTrue(result.GetBool(returnName), "Expected " + comparison + " to return true.");
            return slate;
        }

        #endregion
        #region Action Result...

        /// <summary>Checks if the names in the output are what is expected.</summary>
        /// <param name="result">The results to check the names with.</param>
        /// <param name="names">The names to check against.</param>
        static public Result CheckNames(this Result result, string names) {
            Assert.AreEqual(names, result.OutputNames.Join(", "), "Checking the names in the output.");
            return result;
        }

        /// <summary>Gets the message for the CheckValues assertions.</summary>
        /// <param name="type">The type of the value being checked.</param>
        /// <param name="name">The name of the variable to get.</param>
        /// <returns>The message to show in the assertion.</returns>
        static private string checkValueMsg(string type, string name) =>
            "Checking the " + type + " value of \"" + name + "\".";

        /// <summary>Checks the boolean value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected boolean value.</param>
        /// <param name="name">The name of the variable to get.</param>
        static public Result CheckValue(this Result result, bool exp, string name) {
            Assert.AreEqual(exp, result.GetBool(name), checkValueMsg("bool", name));
            return result;
        }

        /// <summary>Checks the integer value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected integer value.</param>
        /// <param name="name">The name of the variable to get.</param>
        static public Result CheckValue(this Result result, int exp, string name) {
            Assert.AreEqual(exp, result.GetInt(name), checkValueMsg("int", name));
            return result;
        }

        /// <summary>Checks the double value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected double value.</param>
        /// <param name="name">The name of the variable to get.</param>
        static public Result CheckValue(this Result result, double exp, string name) {
            Assert.AreEqual(exp, result.GetDouble(name), checkValueMsg("double", name));
            return result;
        }

        /// <summary>Checks the string value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected string value.</param>
        /// <param name="name">The name of the variable to get.</param>
        static public Result CheckValue(this Result result, string exp, string name) {
            Assert.AreEqual(exp, result.GetString(name), checkValueMsg("string", name));
            return result;
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
