using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Optimization;

/// <summary>The arguments for rules of optimization.</summary>
internal class RuleArgs {

    /// <summary>The slate the formula is for.</summary>
    public readonly Slate Slate;

    /// <summary>The logger to debug and inspect the optimization.</summary>
    /// <remarks>May be null to not log during optimization.</remarks>
    public readonly Logger? Logger;

    /// <summary>The new nodes for a formula which need to be optimized.</summary>
    public readonly HashSet<INode> Nodes;

    /// <summary>The new nodes that will be removed when the rule ends.</summary>
    public readonly HashSet<INode> Removed;

    /// <summary>The root node of the tree to optimize.</summary>
    public INode Root;

    /// <summary>Indicates optimization has changed and a second pass needs to e run.</summary>
    public bool Changed;

    /// <summary>Creates a new rule arguments.</summary>
    /// <param name="slate">The slate the formula is for.</param>
    /// <param name="root">The root node of the tree to optimize.</param>
    /// <param name="nodes">The new nodes for a formula which need to be optimized.</param>
    /// <param name="logger">The logger to debug and inspect the optimization.</param>
    public RuleArgs(Slate slate, INode root, HashSet<INode> nodes, Logger? logger = null) {
        Slate = slate;
        Logger = logger;
        Nodes = nodes;
        Removed = new HashSet<INode>();
        Root = root;
        Changed = false;
    }

    /// <summary>
    /// This evaluates down the branch starting from the given node
    /// to get the correct value prior to converting it to a constant.
    /// </summary>
    /// <param name="node">The node to evaluate.</param>
    public void UpdateValue(INode node) =>
        PostReachable(node).OfType<IEvaluable>().Foreach(eval => eval.Evaluate());

    /// <summary>This will enumerate all new nodes which are reachable from the given node.</summary>
    /// <param name="node">The node to start enumerating from.</param>
    /// <returns>The nodes outputted child before parent.</returns>
    public IEnumerable<INode> PostReachable(INode node) {
        if (node is IChild child) {
            // Copy parents into a list so they can be modified and
            // filter from the list any which are no longer in the nodes.
            foreach (INode parent in child.Parents.ToList().
                Where(Nodes.Contains).WhereNot(Removed.Contains).
                SelectMany(PostReachable))
                yield return parent;
        }
        yield return node;
    }

    /// <summary>This will enumerate all new nodes which are reachable from the given node.</summary>
    /// <param name="node">The node to start enumerating from.</param>
    /// <returns>The nodes outputted child after parent.</returns>
    public IEnumerable<INode> PreReachable(INode node) {
        yield return node;
        if (node is IChild child) {
            // Copy parents into a list so they can be modified and
            // filter from the list any which are no longer in the nodes.
            foreach (INode parent in child.Parents.ToList().
                Where(Nodes.Contains).WhereNot(Removed.Contains).
                SelectMany(PreReachable))
                yield return parent;
        }
    }

    /// <summary>Replace will replace the old node with the new node in all of it's children and in the arguments.</summary>
    /// <param name="oldNode">The old node to replace with the given new node.</param>
    /// <param name="newNode">The new node to replace the old node with.</param>
    public void Replace(INode oldNode, INode newNode) {
        if (ReferenceEquals(Root, oldNode)) Root = newNode;
        else if (oldNode is IParent oldParent && newNode is IParent newParent)
            oldParent.Children.ToList().ForEach(child => child.Parents.Replace(oldParent, newParent));
        Nodes.Add(newNode);
        Removed.Add(oldNode);
        Changed = true;
    }
}
