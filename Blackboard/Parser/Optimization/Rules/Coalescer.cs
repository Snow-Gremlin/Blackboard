using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>An optimizer rule for coalescing parents in nodes which implement ICoalescable.</summary>
    sealed internal class Coalescer : IRule {

        /// <summary>
        /// Find any parents of the same type of node with the given node as its only child
        /// and replace that parent with the parent parents at the same location and in the same order.
        /// </summary>
        /// <param name="node">The node to incorporate parents into.</param>
        static private void incorporateParents(ICoalescable node) {
            IParentCollection parents = node.Parents;
            for (int i = 0; i < parents.Count; ++i) {
                IParent parent = parents[i];

                if (node.GetType().Equals(parent.GetType()) &&
                    parent.Children.IsCount(1) &&
                    ReferenceEquals(parent.Children.First(), node)) {

                    // Incorporate the parent's parents in place of the parent.
                    // Back up the index, so that the new parents are checked.
                    parents.Remove(i);
                    if (parent is IChild childParent)
                        parents.Insert(i, childParent.Parents.Nodes, childParent);
                    --i;
                }
            }
        }

        /// <summary>
        /// This attempts to remove as many parents as possible from the given node by finding constant parents.
        /// The constant parents will be precomputed and any identity parents will be removed.
        /// If there are no parents left then the identity will be returned.
        /// </summary>
        /// <param name="node">The node to try and reduce.</param>
        /// <returns>A node to replace this node with or null to not replace.</returns>
        static private INode commutativeReduce(ICoalescable node) {
            IConstant identity = node.Identity;
            IParentCollection parents = node.Parents;
            if (parents.Count <= 0) return identity;

            List<IParent> remainder = parents.Nodes.
                WhereNot(parent => parent.IsConstant()).
                ToList();
            if (remainder.Count == parents.Count) return null;
            
            List<IParent> constants = parents.Nodes.
                Where(parent => parent.IsConstant()).
                WhereNot(parent => parent.SameValue(identity)).
                ToList();
            if (constants.Count > 1) {
                ICoalescable temp = node.NewInstance();
                temp.Parents.SetAll(constants);
                IConstant constant = temp.ToConstant();
                if (constant is not null && constant is IParent constParent) {
                    constants.Clear();
                    if (!constParent.SameValue(identity))
                        constants.Add(constParent);
                }
            }
            if (remainder.Count + constants.Count == parents.Count) return null;

            remainder.AddRange(constants);
            if (remainder.Count <= 0) return identity;
            node.Parents.SetAll(remainder);
            return null;
        }

        /// <summary></summary>
        /// <param name="node">The node to try and reduce.</param>
        /// <returns>A node to replace this node with or null to not replace.</returns>
        static private INode notcommutableReduce(ICoalescable node) {
            
            // TODO: Implement

            return null;
        }

        /// <summary>Performs a coalesce on the given node.</summary>
        /// <param name="node">The node to reduce and simplify.</param>
        /// <returns>A node to replace this node with or null to not replace.</returns>
        static private INode coalesce(ICoalescable node) {
            if (node.ParentIncorporate) incorporateParents(node);
            return !node.ParentReducable ? null :
                node.Commutative ? commutativeReduce(node) :
                notcommutableReduce(node);
        }

        /// <summary>Reduce the parents and coalesce nodes as much as possible.</summary>
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
                        child.Parents.Replace(cParent, newParent);
                }
            }
        }
    }
}
