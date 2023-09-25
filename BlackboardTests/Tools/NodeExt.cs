using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace BlackboardTests.Tools;

/// <summary>These are extensions to Blackboard Nodes for testing.</summary>
static class NodeExt {

    /// <summary>This will check that the underlying type of the given node is as expected.</summary>
    /// <param name="node">The node to check the type of.</param>
    /// <param name="exp">The expected type of the given node.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckTypeOf(this INode node, Type exp) {
        Assert.AreEqual(exp, Type.TypeOf(node));
        return node;
    }

    /// <summary>Checks the expected cast information between the given node and the given type.</summary>
    /// <param name="node">The node to try to cast into the given type.</param>
    /// <param name="t">The type to try to cast the node into.</param>
    /// <param name="expMatch">The expected results of a type match.</param>
    /// <param name="expImplicit">The node string for the implicit cast node.</param>
    /// <param name="expExplicit">The node string for the explicit cast node.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static private INode checkCasts(INode node, Type t, string expMatch, string expImplicit, string expExplicit) {
        string msg = "Checking if " + t + " matches " + node + ".";
        Assert.AreEqual(expMatch, t.Match(Type.TypeOf(node), true).ToString(), msg);
        Assert.AreEqual(expImplicit, Stringifier.Shallow(t.Implicit(node)), msg);
        Assert.AreEqual(expExplicit, Stringifier.Shallow(t.Explicit(node)), msg);
        return node;
    }

    /// <summary>Checks that there is an implicit cast between the given node and the given type.</summary>
    /// <param name="node">The node to try to cast into the given type.</param>
    /// <param name="t">The type to try to cast the node into.</param>
    /// <param name="steps">The expected implicit distance for the type match.</param>
    /// <param name="expImplicit">The node string for the implicit cast node.</param>
    static public INode CheckImplicit(this INode node, Type t, int steps, string expImplicit) =>
        checkCasts(node, t, "Implicit("+steps+")", expImplicit, "null");

    /// <summary>Checks that there is an inheritance between the given node and the given type.</summary>
    /// <param name="node">The node to try to cast into the given type.</param>
    /// <param name="t">The type to try to cast the node into.</param>
    /// <param name="steps">The expected inherit distance for the type match.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckInherit(this INode node, Type t, int steps) =>
        checkCasts(node, t, "Inherit("+steps+")", Stringifier.Shallow(node), Stringifier.Shallow(node));

    /// <summary>Checks that there is no cast between the given node and the given type.</summary>
    /// <param name="node">The node to try to cast into the given type.</param>
    /// <param name="t">The type to try to cast the node into.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckNoCast(this INode node, Type t) =>
        checkCasts(node, t, "None", "null", "null");

    /// <summary>Checks that there is an explicit cast between the given node and the given type.</summary>
    /// <param name="node">The node to try to cast into the given type.</param>
    /// <param name="t">The type to try to cast the node into.</param>
    /// <param name="expExplicit">The node string for the explicit cast node.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckExplicit(this INode node, Type t, string expExplicit) =>
        checkCasts(node, t, "Explicit", "null", expExplicit);

    /// <summary>Checks the string of a node.</summary>
    /// <param name="node">The node to check the sting of.</param>
    /// <param name="exp">This is the expected string name.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckString(this INode node, string exp) {
        Assert.AreEqual(exp, Stringifier.Shallow(node));
        return node;
    }

    /// <summary>Checks the depth of the node.</summary>
    /// <param name="node">The node to check the depth of.</param>
    /// <param name="exp">This is the expected depth.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public IEvaluable CheckDepth(this IEvaluable node, int exp) {
        Assert.AreEqual(exp, node.Depth);
        return node;
    }

    /// <summary>Checks the list of parents of a node.</summary>
    /// <param name="node">The node to check the parents of.</param>
    /// <param name="exp">The comma separated list of parent strings.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public IChild CheckParents(this IChild node, string exp) {
        Assert.AreEqual(exp, node.Parents.Join(", "));
        return node;
    }

    /// <summary>Checks the boolean value of this node.</summary>
    /// <param name="node">This is the boolean node to check the value of.</param>
    /// <param name="exp">The expected boolean value.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckValue(this INode node, bool exp) {
        Assert.IsInstanceOfType(node, typeof(IValue<Bool>));
        Assert.AreEqual(exp, (node as IValue<Bool>)?.Value.Value);
        return node;
    }

    /// <summary>Checks the integer value of this node.</summary>
    /// <param name="node">This is the integer node to check the value of.</param>
    /// <param name="exp">The expected integer value.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckValue(this INode node, int exp) {
        Assert.IsInstanceOfType(node, typeof(IValue<Int>));
        Assert.AreEqual(exp, (node as IValue<Int>)?.Value.Value);
        return node;
    }

    /// <summary>Checks the double value of this node.</summary>
    /// <param name="node">This is the double node to check the value of.</param>
    /// <param name="exp">The expected double value.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckValue(this INode node, double exp) {
        Assert.IsInstanceOfType(node, typeof(IValue<Double>));
        Assert.AreEqual(exp, (node as IValue<Double>)?.Value.Value);
        return node;
    }

    /// <summary>Checks the string value of this node.</summary>
    /// <param name="node">This is the string node to check the value of.</param>
    /// <param name="exp">The expected string value.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckValue(this INode node, string exp) {
        Assert.IsInstanceOfType(node, typeof(IValue<String>));
        Assert.AreEqual(exp, (node as IValue<String>)?.Value.Value);
        return node;
    }

    /// <summary>Checks the provoked state of this node.</summary>
    /// <param name="node">This is the trigger node to check the state of.</param>
    /// <param name="exp">The expected provoked state of the node.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode CheckProvoked(this INode node, bool exp) {
        Assert.IsInstanceOfType(node, typeof(ITrigger));
        Assert.AreEqual(exp, (node as ITrigger)?.Provoked);
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

    /// <summary>Gets an eval pending list of all the evaluable nodes from the given nodes.</summary>
    /// <param name="nodes">The nodes to get the eval pending from.</param>
    /// <returns>The eval pending of evaluable nodes.</returns>
    static public EvalPending ToEvalPending(this IEnumerable<INode> nodes) {
        EvalPending pending = new();
        pending.Insert(nodes.NotNull().OfType<IEvaluable>());
        return pending;
    }

    /// <summary>Will assign all the children reachable via parents to the parents.</summary>
    /// <param name="node">The node to start adding to the parents from.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode LegitimatizeAll(this INode node) {
        node.GetAllParents().OfType<IChild>().Foreach(child => child.Legitimatize());
        return node;
    }

    /// <summary>Will perform depth update on all the nodes reachable from the parents.</summary>
    /// <param name="node">The node to start updating from and to get all parents and parent parents from.</param>
    /// <param name="logger">Optional logger to use to debug the update with.</param>
    /// <returns>The given node so that method calls can be chained.</returns>
    static public INode UpdateAllParents(this INode node, Logger? logger = null) {
        node.GetAllParents().ToEvalPending().UpdateDepths(logger);
        return node;
    }

    /// <summary>Will perform evaluation on all the nodes reachable from the parents.</summary>
    /// <param name="node">The node to start evaluate from and to get all parents and parent parents from.</param>
    /// <param name="finalization">The set of the triggers which have been provoked and need to be reset.</param>
    /// <param name="logger">Optional logger to use to debug the update with.</param>
    static public void EvaluateAllParents(this INode node, Finalization finalization, Logger? logger = null) =>
        node.GetAllParents().ToEvalPending().Evaluate(finalization, logger);
}
