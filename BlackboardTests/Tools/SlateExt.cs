using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Formuila;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Record;
using Blackboard.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BlackboardTests.Tools;

/// <summary>These are extensions to Blackboard Slate for testing.</summary>
static class SlateExt {

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
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckValue(this Slate slate, bool exp, params string[] names) {
        Assert.AreEqual(exp, slate.GetBool(names), checkValueMsg("bool", names));
        return slate;
    }

    /// <summary>Checks the integer value of this node.</summary>
    /// <param name="slate">This is the slate to check the value with.</param>
    /// <param name="exp">The expected integer value.</param>
    /// <param name="names">The name of the variable to look up.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckValue(this Slate slate, int exp, params string[] names) {
        Assert.AreEqual(exp, slate.GetInt(names), checkValueMsg("int", names));
        return slate;
    }

    /// <summary>Checks the integer value of this node.</summary>
    /// <param name="slate">This is the slate to check the value with.</param>
    /// <param name="exp">The expected integer value.</param>
    /// <param name="names">The name of the variable to look up.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckValue(this Slate slate, uint exp, params string[] names) {
        Assert.AreEqual(exp, slate.GetUint(names), checkValueMsg("uint", names));
        return slate;
    }

    /// <summary>Checks the double value of this node.</summary>
    /// <param name="slate">This is the slate to check the value with.</param>
    /// <param name="exp">The expected double value.</param>
    /// <param name="names">The name of the variable to look up.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckValue(this Slate slate, double exp, params string[] names) {
        Assert.AreEqual(exp, slate.GetDouble(names), checkValueMsg("double", names));
        return slate;
    }

    /// <summary>Checks the float value of this node.</summary>
    /// <param name="slate">This is the slate to check the value with.</param>
    /// <param name="exp">The expected float value.</param>
    /// <param name="names">The name of the variable to look up.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckValue(this Slate slate, float exp, params string[] names) {
        Assert.AreEqual(exp, slate.GetFloat(names), checkValueMsg("float", names));
        return slate;
    }

    /// <summary>Checks the string value of this node.</summary>
    /// <param name="slate">This is the slate to check the value with.</param>
    /// <param name="exp">The expected string value.</param>
    /// <param name="names">The name of the variable to look up.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckValue(this Slate slate, string exp, params string[] names) {
        Assert.AreEqual(exp, slate.GetString(names), checkValueMsg("string", names));
        return slate;
    }

    /// <summary>Checks the object value of this node.</summary>
    /// <param name="slate">This is the slate to check the value with.</param>
    /// <param name="exp">The expected object value.</param>
    /// <param name="names">The name of the variable to look up.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckObject(this Slate slate, object exp, params string[] names) {
        Assert.AreEqual(exp, slate.GetObject(names), checkValueMsg("object", names));
        return slate;
    }

    /// <summary>Checks the object value of this node.</summary>
    /// <param name="slate">This is the slate to check the object with.</param>
    /// <param name="exp">The expected object value.</param>
    /// <param name="names">The name of the variable to look up.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckValueAsObject(this Slate slate, object exp, params string[] names) {
        Assert.AreEqual(exp, slate.GetValueAsObject(names), checkValueMsg("object", names));
        return slate;
    }

    /// <summary>Checks the provoked state of this node.</summary>
    /// <param name="slate">This is the slate to check the state with.</param>
    /// <param name="exp">The expected provoked state of the node.</param>
    /// <param name="names">The name of the variable to look up.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckProvoked(this Slate slate, bool exp, params string[] names) {
        Assert.AreEqual(exp, slate.Provoked(names), "Checking the provoked state of \"" + names.Join(".") + "\".");
        return slate;
    }

    /// <summary>Checks the pending nodes to evaluate are as expected.</summary>
    /// <param name="slate">This is the slate to check the pending of.</param>
    /// <param name="exp">The expected names of the pending nodes.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckPendingEval(this Slate slate, params string[] exp) {
        TestTools.NoDiff(exp, slate.PendingEval.Strings(), "Checking the pending nodes.");
        return slate;
    }

    /// <summary>Checks the pending nodes to update are as expected.</summary>
    /// <param name="slate">This is the slate to check the pending of.</param>
    /// <param name="exp">The expected names of the pending nodes.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckPendingUpdate(this Slate slate, params string[] exp) {
        TestTools.NoDiff(exp, slate.PendingUpdate.Strings(), "Checking the pending nodes.");
        return slate;
    }

    /// <summary>Runs the slate evaluation and checks that evaluation performed as expected.</summary>
    /// <param name="slate">The slate to evaluate.</param>
    /// <param name="lines">The expected evaluation log output.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckEvaluate(this Slate slate, params string[] lines) {
        BufferLogger logger = new(false);
        slate.PerformEvaluation(logger.Stringify(Stringifier.Shallow().PreLoadNames(slate)));
        TestTools.NoDiff(lines.Join("\n"), logger.ToString().Trim());
        return slate;
    }

    /// <summary>Runs the slate update and checks that evaluation performed as expected.</summary>
    /// <param name="slate">The slate to evaluate.</param>
    /// <param name="lines">The expected evaluation log output.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckUpdate(this Slate slate, params string[] lines) {
        BufferLogger logger = new(false);
        slate.PerformUpdates(logger.Stringify(Stringifier.Shallow().PreLoadNames(slate)));
        TestTools.NoDiff(lines.Join("\n"), logger.ToString().Trim());
        return slate;
    }

    /// <summary>Checks the string for the given node using the names from the given slate.</summary>
    /// <param name="slate">The slate to load the names for the nodes from.</param>
    /// <param name="stringifier">The stringifier to stringify with.</param>
    /// <param name="node">The node to get the string for.</param>
    /// <param name="lines">The line to compare the node's string against.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckNodeString(this Slate slate, Stringifier stringifier, INode node, params string[] lines) {
        stringifier.PreLoadNames(slate);
        TestTools.NoDiff(lines.Join("\n"), stringifier.Stringify(node));
        return slate;
    }

    /// <summary>Checks the string for the given node using the names from the given slate.</summary>
    /// <param name="slate">The slate to load the names for the nodes from.</param>
    /// <param name="stringifier">The stringifier to stringify with.</param>
    /// <param name="nodeName">The name of the node to get the string for.</param>
    /// <param name="lines">The line to compare the node's string against.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckNodeString(this Slate slate, Stringifier stringifier, string nodeName, params string[] lines) =>
        slate.CheckNodeString(stringifier, slate.GetNode<INode>(nodeName), lines);

    /// <summary>Checks the deep string for the given node using the names from the given slate.</summary>
    /// <param name="slate">The slate to load the names for the nodes from.</param>
    /// <param name="nodeName">The name of the node to get the deep string for.</param>
    /// <param name="lines">The line to compare the node's string against.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckNodeString(this Slate slate, INode node, params string[] lines) =>
        slate.CheckNodeString(Stringifier.Deep(), node, lines);

    /// <summary>Checks the deep string for the given node using the names from the given slate.</summary>
    /// <param name="slate">The slate to load the names for the nodes from.</param>
    /// <param name="nodeName">The name of the node to get the deep string for.</param>
    /// <param name="lines">The line to compare the node's string against.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckNodeString(this Slate slate, string nodeName, params string[] lines) =>
        slate.CheckNodeString(slate.GetNode<INode>(nodeName), lines);

    /// <summary>Checks the namespace string for the whole graph and compares against the given lines.</summary>
    /// <param name="slate">The slate to compare against.</param>
    /// <param name="lines">The expected lines of the returned string.</param>
    /// <returns>The slate so that method calls can be chained together.</returns>
    static public Slate CheckGraphString(this Slate slate, params string[] lines) {
        TestTools.NoDiff(lines.Join("\n"), Stringifier.GraphString(slate));
        return slate;
    }

    /// <summary>Performs a parse of the given input and returns the formula for the input.</summary>
    /// <param name="slate">The slate to apply the parsed formula to.</param>
    /// <param name="input">The lines of the code to read and get the formula for.</param>
    /// <remarks>The formula for the read input.</remarks>
    static public Formula Read(this Slate slate, params string[] input) =>
        new Parser(slate).Read(input);

    /// <summary>Performs a parse of the given input and logs the parse output.</summary>
    /// <param name="slate">The slate to apply the parsed formula to.</param>
    /// <param name="input">The lines of the code to read and commit.</param>
    static public Formula LogRead(this Slate slate, params string[] input) {
        Stringifier stringifier = Stringifier.Deep().PreLoadNames(slate);
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
}
