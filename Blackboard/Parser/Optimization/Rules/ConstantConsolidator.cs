using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>An optimizer rule for consolidating constants in nodes which implement ICoalescable.</summary>
    sealed internal class ConstantConsolidator: IRule {

        /// <summary>Gets the name of this rule.</summary>
        /// <returns>The string for this class used as the name of the rule.</returns>
        public override string ToString() => nameof(ConstantConsolidator);

        /// <summary>
        /// If there are more than one non-identity constant, then precompute the result of this node
        /// being applied to those constants. If the constants can be reduced, then it will be returned
        /// via the constants, unless it is the identity then the constants list will be empty.
        /// </summary> 
        /// <param name="args">The arguments for the optimization rules.</param>
        /// <param name="node">The node these constants are parents of.</param>
        /// <param name="identity">The identity value for this node.</param>
        /// <param name="constants">The set of constants to reduce.</param>
        /// <returns>The reduced set of constants.</returns>
        static private List<IParent> reduceConstants(RuleArgs args, ICoalescable node, IConstant identity, List<IParent> constants) {
            if (constants.Count <= 1) return constants;

            // Create a new instance of the current node, set the constants as the
            // only parents, evaluate that node, then get the result as a constant.
            ICoalescable temp = node.NewInstance() as ICoalescable;
            temp.Parents.SetAll(constants);
            args.UpdateValue(temp);
            IConstant constant = temp.ToConstant();

            // If the reduced constant is valid, then replace the constants with the reduced constant.
            // If the reduced constant is the identity, then return no constants.
            if (constant is not null && constant is IParent constParent) {
                constants.Clear();
                if (!constParent.SameValue(identity)) {
                    constants.Add(constParent);
                    args.Removed.Add(constParent);
                }
                args.Changed = true;
                args.Logger.Info("  Consolidate {0} => {1}", temp, constant);
            }
            return constants;
        }

        /// <summary>
        /// This attempts to remove as many parents as possible from the given node by finding constant parents.
        /// The constant parents will be precomputed and any identity parents will be removed.
        /// If there are no parents left then the identity will be returned.
        /// </summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        /// <param name="node">The node to try and reduce.</param>
        /// <returns>A node to replace this node with or null to not replace.</returns>
        static private INode commutativeReduce(RuleArgs args, ICoalescable node) {
            IConstant identity = node.Identity;
            IParentCollection parents = node.Parents;

            // If there are no parents then return the identity for this node.
            if (parents.Count <= 0) return identity;

            // Find all parents which are NOT constant.
            List<IParent> remainder = parents.
                WhereNot(parent => parent.IsConstant()).
                ToList();

            // If the non-constant parents are all of the parents, then just leave, there is nothing to do.
            if (remainder.Count == parents.Count) return null;

            // Find all parents which are constant, but skip any constants which are equal to the identity.
            List<IParent> constants = parents.
                Where(parent => parent.IsConstant()).
                WhereNot(parent => parent.SameValue(identity)).
                ToList();

            // Reduce the constants to the smallest set of constants.
            constants = reduceConstants(args, node, identity, constants);

            // If the non-constant parents and the reduced constant parents equal the initial parents,
            // then no parents were able to be reduced, so just leave, there is nothing to do.
            if (remainder.Count + constants.Count == parents.Count) return null;

            // Add the constants into the list of non-constants as the new parents for this node.
            // If there are no parents left at all, then return the identity for the node.
            remainder.AddRange(constants);
            if (remainder.Count <= 0) return identity;
            parents.SetAll(remainder);
            return null;
        }

        /// <summary></summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        /// <param name="node">The node to try and reduce.</param>
        /// <returns>A node to replace this node with or null to not replace.</returns>
        static private INode notcommutableReduce(RuleArgs args, ICoalescable node) {

            // TODO: Implement

            return null;
        }

        /// <summary>Reduce the parents and coalesce constant nodes as much as possible.</summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        public void Perform(RuleArgs args) {
            foreach (ICoalescable node in args.Nodes.OfType<ICoalescable>().WhereNot(args.Removed.Contains).Where(node => node.ParentReducable)) {
                INode newNode = node.Commutative ? commutativeReduce(args, node) : notcommutableReduce(args, node);
                if (newNode is null) continue;
                args.Replace(node, newNode);
            }
        }
    }
}
