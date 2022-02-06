using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>An optimizer rule for coalescing parents in nodes which implement ICoalescable.</summary>
    sealed internal class Coalescer : IRule {

        /// <summary>
        /// Find any parents of the same type of node with the given node as its only child
        /// and replace that parent with the parent parents at the same location and in the same order.
        /// </summary>
        /// <param name="node">The node to incorporate parents into.</param>
        static private void incorporateParents(ICoalescable node) {
            List<IParent> parents = node.Parents.ToList();
            for (int i = 0; i < parents.Count; ++i) {
                IParent parent = parents[i];

                if (node.GetType().Equals(parent.GetType()) &&
                    parent.Children.IsCount(1) &&
                    ReferenceEquals(parent.Children.First(), node)) {

                    // Incorporate the parent's parents here.
                    parents.RemoveAt(i);
                    if (parent is IChild childParent)
                        parents.InsertRange(i, childParent.Parents);
                    --i;
                }
            }

            // TODO: Set Parents
        }

        /// <summary></summary>
        /// <param name="node"></param>
        /// <returns></returns>
        static private INode commutativeCoalesce(ICoalescable node) {

            // TODO: Implement

            return null;
        }


        /// <summary></summary>
        /// <param name="node"></param>
        /// <returns></returns>
        static private INode notcommutableCoalesce(ICoalescable node) {

            // TODO: Implement

            return null;
        }

        /// <summary>Performs a coalesce on the given node.</summary>
        /// <param name="node">The node to reduce and simplify.</param>
        /// <returns>A node to replace this node with or null to not replace.</returns>
        static private INode coalesce(ICoalescable node) {
            if (node.ParentIncorporate) incorporateParents(node);
            return node.Commutative? commutativeCoalesce(node) : notcommutableCoalesce(node);
        }

        /// <summary>Reduce the parents and coalesce nodes as mush as possible.</summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        public void Perform(RuleArgs args) {
            foreach (INode node in args.Nodes) {
                if (node is not ICoalescable cNode) continue;

                INode newNode = coalesce(cNode);
                if (newNode is null) continue;

                if (ReferenceEquals(cNode, args.Root))
                    args.Root = newNode;
                else if (newNode is IParent newParent && cNode is IParent cParent ) {
                    foreach (IChild child in cParent.Children.ToList())
                        child.ReplaceParent(cParent, newParent);
                }
            }
        }
    }
}
