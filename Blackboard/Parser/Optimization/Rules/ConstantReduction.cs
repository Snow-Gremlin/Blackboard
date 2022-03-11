using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using System.Linq;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>
    /// This is an optimization rule for finding constant branches and replacing them with literals.
    /// This may leave nodes further down the constant branch in the nodes set.
    /// This will also look up the stored constant in the slate and use that or
    /// store the constant if one by that value doesn't exist yet.
    /// </summary>
    sealed internal class ConstantReduction: IRule {

        /// <summary>Recursively finds constant branches and replaces the branch with literals.</summary>
        /// <param name="args">The rule arguments used while optimizing.</param>
        /// <param name="node">The node of the tree to constant optimize.</param>
        /// <remarks>The node to replace the given one in the parent or null to not replace.</remarks>
        static private INode reduceNode(RuleArgs args, INode node) {
            // If this node is not part of the new nodes, just return it.
            if (!args.Nodes.Contains(node)) return null;

            // Check if the node can be turned into a constant.
            if (node.IsConstant()) {
                args.UpdateValue(node);
                IConstant con = node.ToConstant();
                if (con is not null) {

                    // Look up constant in slate, if one doesn't exist, this will add it.
                    con = args.Slate.FindConstant(con);
                    if (!ReferenceEquals(con, node)) {
                        args.Logger.Notice("Replace {0} with constant {1}", node, con);
                        args.Changed = true;
                        args.Nodes.Add(con);
                        return con;
                    }
                }
            }

            // If the node is not a child just return the node.
            if (node is not IChild child) return null;

            // Check each parent in the child node.
            IParentCollection parents = child.Parents;
            foreach (IParent parent in parents.Nodes.ToList()) {
                INode newNode = reduceNode(args, parent);
                if (newNode is not null and IParent newParent)
                    parents.Replace(parent, newParent);
            }

            return null;
        }

        /// <summary>Finds all constant branches and replaces each branch with literals.</summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        public void Perform(RuleArgs args) {
            args.Logger.Info("Run "+nameof(ConstantReduction));
            INode newNode = reduceNode(args, args.Root);
            if (newNode is not null) args.Root = newNode;
            args.Logger.Info("Reduction: {0}", args.Root);
        }
    }
}
