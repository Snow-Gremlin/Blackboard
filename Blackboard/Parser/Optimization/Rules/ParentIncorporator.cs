using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using System.Linq;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>An optimizer rule for incorporating parents in nodes which implement ICoalescable.</summary>
    sealed internal class ParentIncorporator : IRule {

        /// <summary>
        /// Find any parents of the same type of node with the given node as its only child
        /// and replace that parent with the parent parents at the same location and in the same order.
        /// </summary>
        /// <param name="node">The node to incorporate parents into.</param>
        static private void incorporateParents(RuleArgs args, ICoalescable node) {
            IParentCollection parents = node.Parents;
            for (int i = 0; i < parents.Count; ++i) {
                IParent parent = parents[i];

                if (node.GetType().Equals(parent.GetType()) &&
                    parent.Children.IsCount(1) &&
                    ReferenceEquals(parent.Children.First(), node) &&
                    parent is IChild childParent) {

                    // Incorporate the parent's parents in place of the parent.
                    // Step back the index, so that the new parents are checked.
                    parents.Remove(i);
                    parents.Insert(i, childParent.Parents.Nodes, childParent);
                    args.Nodes.Remove(childParent);
                    --i;
                    args.Changed = true;
                }
            }
        }

        /// <summary>Incorporates parents as part of the coalesce of nodes.</summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        public void Perform(RuleArgs args) {
            foreach (INode node in args.Nodes) {
                if (node is ICoalescable cNode && cNode.ParentIncorporate)
                    incorporateParents(args, cNode);
            }
        }
    }
}
