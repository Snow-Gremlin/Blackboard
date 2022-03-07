using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>Removes unreachable nodes.</summary>
    sealed internal class RemoveUnreachable: IRule {

        /// <summary>Finds reachable nodes recursively.</summary>
        /// <param name="node">The node to check.</param>
        /// <param name="newNodes">The set of new nodes for the formula.</param>
        /// <param name="reached">The set of the nodes which have been reached.</param>
        private void findReachable(INode node, HashSet<INode> newNodes, HashSet<INode> reached) {
            if (!newNodes.Contains(node)) return;
            reached.Add(node);
            if (node is IChild child) {
                foreach (IParent parent in child.Parents.Nodes)
                    this.findReachable(parent, newNodes, reached);
            }
        }

        /// <summary>Finds unreachable nodes to remove from the given set of nodes.</summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        public void Perform(RuleArgs args) {
            HashSet<INode> reached = new();
            this.findReachable(args.Root, args.Nodes, reached);

            args.Logger.Info("Removed {0} unreachable nodes. {1} nodes were reachable.",
                args.Nodes.Count - reached.Count, reached.Count);
            
            args.Nodes.Clear();
            args.Nodes.UnionWith(reached);
        }
    }
}
