using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Extensions {

    /// <summary>The set of extensions for working with IChild interfaces.</summary>
    static class IChildExt {

        /// <summary>This is a helper method for setting a parent to the node.</summary>
        /// <remarks>This is only intended to be called by the given child internally.</remarks>
        /// <typeparam name="T">The node type for the parent.</typeparam>
        /// <param name="child">The child to set the parent to.</param>
        /// <param name="parent">The parent variable being set.</param>
        /// <param name="newParent">The new parent being set, or null</param>
        static internal void SetParent<T>(this IChild child, ref T parent, T newParent) where T : IParent {
            if (ReferenceEquals(parent, newParent)) return;
            parent?.RemoveChildren(child);
            parent = newParent;
            // Do not add parent yet, so we can read from the parents when only evaluating.
        }

        /// <summary>
        /// Adding this node into the parents will make this node be automatially called when
        /// the parents are updated. This should be done if the node is part of a definition.
        /// Do not do add if the node is only suppose to evaluate, that way we aren't updating what
        /// we don't need to update and aren't constantly adding and removing children from parents.
        /// </summary>
        /// <param name="child">The child to add to the parents.</param>
        static public void AddToParents(this IChild child) {
            foreach (IParent parent in child.Parents)
                parent?.AddChildren(child);
        }

        /// <summary>This will check if from the given root node any of the given target nodes can be reachable.</summary>
        /// <param name="root">The root to start checking from.</param>
        /// <param name="targets">The target nodes to try to reach.</param>
        /// <returns>True if any of the targets can be reached, false otherwise.</returns>
        static public bool CanReachAny(this IChild root, params INode[] targets) =>
            root.CanReachAny(targets as IEnumerable<INode>);

        /// <summary>This will check if from the given root node any of the given target nodes can be reachable.</summary>
        /// <param name="root">The root to start checking from.</param>
        /// <param name="targets">The target nodes to try to reach.</param>
        /// <returns>True if any of the targets can be reached, false otherwise.</returns>
        static public bool CanReachAny(this IChild root, IEnumerable<INode> targets) {
            if (root is null) return true;
            HashSet<IChild> touched = new();
            Queue<IChild> pending = new();
            pending.Enqueue(root);
            touched.Add(root);

            while (pending.Count > 0) {
                IChild node = pending.Dequeue();
                if (targets.Contains(node)) return true;

                foreach (IChild child in node.Parents.NotNull().OfType<IChild>().WhereNot(touched.Contains)) {
                    pending.Enqueue(child);
                    touched.Add(child);
                }
            }
            return false;
        }
    }
}
