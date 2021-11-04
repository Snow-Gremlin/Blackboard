using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for all nodes in the blackboard.</summary>
    public interface INode {

        /// <summary>Gets a string for the given node even if the node is null.</summary>
        /// <param name="node">The node to get a string for.</param>
        /// <returns>The string for the given node.</returns>
        static public string NodeString(INode node) =>
            node is null ? "null" : node.ToString();

        /// <summary>Gets a string got the given set of nodes comma separated.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The string for the given nodes.</returns>
        static public string NodeString(IEnumerable<INode> nodes) =>
            nodes.Select((INode node) => NodeString(node)).Join(", ");

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="node">The node to get a string for.</param>
        /// <param name="scopeName">The name of this node from a parent namespace or empty for no name.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        static public string NodePrettyString(INode node, string scopeName = "", int nodeDepth = int.MaxValue) =>
            node is null ? "null" : node.PrettyString(scopeName, nodeDepth);

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <param name="scopeName">The name of this node from a parent namespace or empty for no name.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        static public string NodePrettyString(IEnumerable<INode> nodes, string scopeName = "", int nodeDepth = int.MaxValue) =>
            nodes.Select((INode node) => NodePrettyString(node, scopeName, nodeDepth)).Join(", ");

        /// <summary>This will check if from the given root node any of the given target nodes can be reachable.</summary>
        /// <param name="root">The root to start checking from.</param>
        /// <param name="targets">The target nodes to try to reach.</param>
        /// <returns>True if any of the targets can be reached, false otherwise.</returns>
        static public bool CanReachAny(INode root, params INode[] targets) =>
            CanReachAny(root, targets as IEnumerable<INode>);

        /// <summary>This will check if from the given root node any of the given target nodes can be reachable.</summary>
        /// <param name="root">The root to start checking from.</param>
        /// <param name="targets">The target nodes to try to reach.</param>
        /// <returns>True if any of the targets can be reached, false otherwise.</returns>
        static public bool CanReachAny(INode root, IEnumerable<INode> targets) {
            List<INode> touched = new();
            Queue<INode> pending = new();
            pending.Enqueue(root);
            while (pending.Count > 0) {
                INode node = pending.Dequeue();
                if (node is null) continue;
                touched.Add(node);
                if (targets.Contains(node)) return true;
                foreach (INode parent in node.Parents) {
                    if (!touched.Contains(parent)) pending.Enqueue(parent);
                }
            }
            return false;
        }

        /// <summary>This is the type name of the node without any type parameters.</summary>
        string TypeName { get; }

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="scopeName">The name of this node from a parent namespace or empty for no name.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        string PrettyString(string scopeName = "", int nodeDepth = int.MaxValue);

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        IEnumerable<INode> Parents { get; }

        /// <summary>The set of children nodes to this node in the graph.</summary>
        IEnumerable<INode> Children { get; }
    }
}
