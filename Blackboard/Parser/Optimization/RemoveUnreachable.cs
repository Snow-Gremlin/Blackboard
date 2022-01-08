using Blackboard.Core;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Parser.Optimization {

    /// <summary>Removes unreachable nodes.</summary>
    sealed internal class RemoveUnreachable: IRule {

        /// <summary>Creates a new unreachable rule.</summary>
        public RemoveUnreachable() { }

        /// <summary>Finds reachable nodes recursively.</summary>
        /// <param name="node">The node to check.</param>
        /// <param name="newNodes">The set of new nodes for the formula.</param>
        /// <param name="reached">The set of the nodes which have been reached.</param>
        public void findReachable(INode node, HashSet<INode> newNodes, HashSet<INode> reached) {
            if (!newNodes.Contains(node)) return;
            reached.Add(node);
            if (node is IChild child) {
                foreach (IParent parent in child.Parents)
                    this.findReachable(parent, newNodes, reached);
            }
        }

        /// <summary>Finds unreachable nodes to remove from the given set of nodes.</summary>
        /// <param name="slate">The slate the formula is for.</param>
        /// <param name="root">The root node of the tree to optimize.</param>
        /// <param name="nodes">The formula nodes to optimize.</param>
        /// <param name="logger">The logger to debug and inspect the optimization.</param>
        /// <remarks>The node to replace the given one in the parent or null to not replace.</remarks>
        public INode Perform(Slate slate, INode root, HashSet<INode> nodes, ILogger logger = null) {
            HashSet<INode> reached = new();
            this.findReachable(root, nodes, reached);
            logger?.Log("Removed {0} unreachable nodes. {1} nodes were reachable.", nodes.Count - reached.Count, reached.Count);
            nodes.Clear();
            nodes.UnionWith(reached);
            return null;
        }
    }
}
