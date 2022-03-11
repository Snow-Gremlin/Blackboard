using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>Removes unreachable nodes.</summary>
    sealed internal class RemoveUnreachable: IRule {

        /// <summary>Finds reachable nodes recursively.</summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        /// <param name="node">The node to check.</param>
        /// <param name="reached">The set of the nodes which have been reached.</param>
        static private void findReachable(RuleArgs args, INode node, HashSet<INode> reached) {
            if (!args.Nodes.Contains(node)) return;
            reached.Add(node);
            if (node is IChild child) {
                foreach (IParent parent in child.Parents.Nodes)
                    findReachable(args, parent, reached);
            }
        }

        /// <summary>Finds unreachable nodes to remove from the given set of nodes.</summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        public void Perform(RuleArgs args) {
            args.Logger.Info("Run "+nameof(RemoveUnreachable));
            HashSet<INode> reached = new();
            findReachable(args, args.Root, reached);

            args.Logger.Info("Removed {0} unreachable nodes. {1} nodes were reachable.",
                args.Nodes.Count - reached.Count, reached.Count);

            foreach ((INode node, int index) in args.Nodes.WithIndex())
                args.Logger.Info("{0}) [{1}] {2}", index, reached.Contains(node) ? "X" : " ", node);
            
            args.Nodes.Clear();
            args.Nodes.UnionWith(reached);
        }
    }
}
