using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Collections;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>A child node is a node which may specify one or more parents.</summary>
    /// <remarks>
    /// It is up to the node to define how the parents are set since the parents are
    /// the input values into a node so there may be a specific number of them and
    /// they may be required to be a specific type of nodes.
    /// </remarks>
    public interface IChild: INode {

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

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
        /// </remarks>
        public ParentCollection Parents { get; }
    }
}
