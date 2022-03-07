using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>
    /// This is an optimization rule for finding constant branches and replacing them with literals.
    /// This may leave nodes further down the constant branch in the nodes set.
    /// This will also look up the stored constant in the slate and use that or
    /// store the constant if one by that value doesn't exist yet.
    /// </summary>
    sealed internal class ConstantReduction: IRule {

        /// <summary>
        /// This evaluates down the branch starting from the given node
        /// to get the correct value prior to converting it to a constant.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <param name="nodes">The new nodes for a formula.</param>
        private void updateValue(INode node, HashSet<INode> nodes) {
            if (!nodes.Contains(node)) return;
            if (node is IChild child) {
                foreach (INode parent in child.Parents.Nodes)
                    this.updateValue(parent, nodes);
            }
            if (node is IEvaluable eval)
                eval.Evaluate();
        }

        /// <summary>Recursively finds constant branches and replaces the branch with literals.</summary>
        /// <param name="slate">The slate the formula is for.</param>
        /// <param name="node">The node of the tree to constant optimize.</param>
        /// <param name="nodes">The formula nodes to optimize.</param>
        /// <param name="logger">The logger to debug and inspect the optimization.</param>
        /// <remarks>The node to replace the given one in the parent or null to not replace.</remarks>
        private INode reduceNode(Slate slate, INode node, HashSet<INode> nodes, Logger logger) {
            // If this node is not part of the new nodes, just return it.
            if (!nodes.Contains(node)) return null;

            // Check if the node can be turned into a constant.
            if (node.IsConstant()) {
                this.updateValue(node, nodes);
                IConstant con = node.ToConstant();
                if (con is not null) {

                    // Look up constant in slate, if one doesn't exist, this will add it.
                    con = slate.FindConstant(con);
                    if (!ReferenceEquals(con, node)) {
                        logger.Info("Replace {0} with constant {1}", node, con);
                        return con;
                    }
                }
            }

            // If the node is not a child just return the node.
            if (node is not IChild child) return null;

            // Check each parent in the child node.
            IParentCollection parents = child.Parents;
            foreach (IParent parent in parents.Nodes.ToList()) {
                INode newNode = this.reduceNode(slate, parent, nodes, logger);
                if (newNode is not null and IParent newParent)
                    parents.Replace(parent, newParent);
            }

            return null;
        }

        /// <summary>Finds all constant branches and replaces each branch with literals.</summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        public void Perform(RuleArgs args) {
            INode newNode = this.reduceNode(args.Slate, args.Root, args.Nodes, args.Logger);
            if (newNode is not null) args.Root = newNode;
        }
    }
}
