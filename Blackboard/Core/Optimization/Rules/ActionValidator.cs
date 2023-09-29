using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Interfaces;
using System.Linq;

namespace Blackboard.Core.Optimization.Rules;

/// <summary>Validates all the nodes in the action being created.</summary>
sealed internal class ActionValidator : IRule {

    /// <summary>Checks all the children of the given parent.</summary>
    /// <param name="args">The argument for this rule.</param>
    /// <param name="parent">The parent node to check all the children of.</param>
    static private void checkChildren(RuleArgs args, IParent parent) {
        foreach ((IChild child, int index) in parent.Children.WithIndex()) {
            if (child is null) {
                // A null child should not exist in the parent's children list.
                args.Logger.Error(new Message("A new parent node should not have a null child.").
                    With("Parent", parent).
                    With("Child Index", index));
                continue;
            }

            if (args.Nodes.Contains(child)) {
                // The child is a new node of a new parent child. These nodes should be fully connected to each other.
                // We only need to check if the child contains the parent since we know the parent contains the child to reach here.
                if (!child.Parents.Contains(parent))
                    args.Logger.Error(new Message("A new node child of a new node must have that parent in its parent list.").
                        With("Child", child).
                        With("Parent", parent).
                        With("Child Index", index));
                continue;
            }

            // The child is an old node of a new parent. The new parent should not have any old children.
            // There shouldn't be any way that this should happen so just error.
            args.Logger.Error(new Message("Should not have an old child of a new node.").
                With("Child", child).
                With("Parent", parent).
                With("Child Index", index));
        }
    }

    /// <summary>Checks all the parents of the given child.</summary>
    /// <param name="args">The argument for this rule.</param>
    /// <param name="child">The child node to check all the parents of.</param>
    static private void checkParent(RuleArgs args, IChild child) {
        int nonNullCount = 0;
        foreach ((IParent parent, int index) in child.Parents.WithIndex()) {
            if (parent is null) {
                // This shouldn't be able to happen because parent collections should filter them.
                args.Logger.Error(new Message("A new child should not have a null parent.").
                    With("Child", child).
                    With("Parent Index", index));
                continue;
            }

            nonNullCount++;
            if (args.Nodes.Contains(parent)) {
                // The parent is a new node with a new node child. These nodes should be fully connected to each other.
                // We only need to check if the parent contains the child since we know the child contains the parent to reach here.
                if (!parent.Children.Contains(child))
                    args.Logger.Error(new Message("A new node parent of a new node must have that child in its children list.").
                        With("Child", child).
                        With("Parent", parent).
                        With("Parent Index", index));
                continue;
            }

            // The parent is an old node with a new node child. Since the action that is being created has not been
            // run yet, old nodes should not know about the new nodes. That way if the action is deleted then the
            // new nodes will not be references and will also be disposed of.
            if (parent.Children.Contains(child))
                args.Logger.Error(new Message("A new child should be illegitimate and unknown by old parents.").
                    With("Child", child).
                    With("Parent", parent).
                    With("Parent Index", index));
        }

        // Since null parents should be filtered, check if the full count which shouldn't remove nulls,
        // against the amount of non-null parents which were checked above.
        if (nonNullCount != child.Parents.Count)
            args.Logger.Error(new Message("A new child contains null parents based on counts.").
                With("Child", child).
                With("Non-null Count", nonNullCount).
                With("Parent Count", child.Parents.Count));
    }

    /// <summary>Performs validation of the action being created.</summary>
    /// <param name="args">The argument for this rule.</param>
    public void Perform(RuleArgs args) {
        foreach (INode node in args.Nodes) {
            if (node is IParent parent) checkChildren(args, parent);
            if (node is IChild child) checkParent(args, child);
        }
    }
}
