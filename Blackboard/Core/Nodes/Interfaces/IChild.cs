using Blackboard.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>A child node is a node which may specify one or more parents.</summary>
    /// <remarks>
    /// It is up to the node to define how the parents are set since the parents are
    /// the input values into a node so there may be a specific number of them and
    /// they may be required to be a specific type of nodes.
    /// </remarks>
    public interface IChild: INode {

        /// <summary>This will enumerate the given nodes which are not null.</summary>
        /// <remarks>This is designed to be used to help return parents as an enumerator.</remarks>
        /// <param name="values">The parent nodes to enumerate.</param>
        /// <returns>The enumerator for the passed in nodes which are not null.</returns>
        static protected IEnumerable<IParent> EnumerateParents(params INode[] values) =>
            values.NotNull().OfType<IParent>();

        /// <summary>This is a helper method for setting a parent to the given node.</summary>
        /// <remarks>
        /// This is only intended to be called by the given child internally.
        /// Do not add this child to the new parent yet,
        /// so we can read from the parents when only evaluating.
        /// This allows nodes which aren't parents for nodes like select.
        /// </remarks>
        /// <typeparam name="T">The node type for the parent.</typeparam>
        /// <param name="child">The child to set the parent to.</param>
        /// <param name="node">The parent node variable being set.</param>
        /// <param name="newParent">The new parent being set, or null</param>
        /// <returns>True if the parent has changed, false otherwise.</returns>
        static protected bool SetParent<T>(IChild child, ref T node, T newParent)
            where T : IParent {
            if (ReferenceEquals(node, newParent)) return false;
            bool removed = node?.RemoveChildren(child) ?? false;
            node = newParent;
            if (removed) node?.AddChildren(child);
            return true;
        }

        /// <summary>This is a helper method for replacing a parent in the given node.</summary>
        /// <typeparam name="T">The node type for the parent.</typeparam>
        /// <param name="child">The child to replace the parent for.</param>
        /// <param name="node">The parent node variable being replaced.</param>
        /// <param name="oldParent">The old parent to check for.</param>
        /// <param name="newParent">The new parent being set, or null</param>
        /// <returns>True if replaced, false if not.</returns>
        static protected bool ReplaceParent<T>(IChild child, ref T node, IParent oldParent, IParent newParent)
            where T : class, IParent {
            if (!ReferenceEquals(node, oldParent)) return false;
            if (newParent is not null and not T)
                throw new Exception("Unable to replace old parent with new parent.").
                    With("child", child).
                    With("node", node).
                    With("old Parent", oldParent).
                    With("new Parent", newParent);
            bool removed = node?.RemoveChildren(child) ?? false;
            node = newParent as T;
            if (removed) node?.AddChildren(child);
            return true;
        }

        /// <summary>This checks if the set of parents about to be set to this node is valid.</summary>
        /// <param name="parents">The list of parents to check before being set.</param>
        /// <param name="variableLength">
        /// True if any number of parents may be set, false to check the count.
        /// If true then there must be at least one type in the expected types.
        /// </param>
        /// <param name="expectedTypes">The expected types for the parents.</param>
        static internal void CheckParentsBeingSet(List<IParent> parents, bool variableLength, params S.Type[] expectedTypes) {
            if (!variableLength) {
                int count = expectedTypes.Length;
                if (parents.Count != count)
                    throw new Exception("Incorrect number of parents in the list of parents to set to a node.").
                        With("Expected Count", count).
                        With("Given Count", parents.Count);
            }

            int index = 0;
            foreach ((IParent parent, S.Type expectedType) in parents.Zip(expectedTypes)) {
                if (parent.GetType().IsAssignableTo(expectedType))
                    throw new Exception("Incorrect type of a parent in the list of parents to set to a node.").
                        With("Index", index).
                        With("Expected Type", expectedType).
                        With("Gotten Type", parent.GetType());
                ++index;
            }
        }

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
        /// </remarks>
        public IEnumerable<IParent> Parents { get; }

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <remarks>
        /// The new parent must be able to take the place of the old parent,
        /// otherwise this will throw an exception when attempting the replacement of the old parent.
        /// </remarks>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool ReplaceParent(IParent oldParent, IParent newParent);

        /// <summary>This will attempt to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAllParents(List<IParent> newParents);
    }
}
