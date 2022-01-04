using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Optimization {

    /// <summary>
    /// This is an optimization rule for finding constant branches and replacing them with literals.
    /// This may leave nodes further down the constant branch in the nodes set.
    /// </summary>
    internal class ConstantReduction: IRule {

        /// <summary>Creates a new constant reduction rule.</summary>
        public ConstantReduction() { }

        /// <summary>Recursively finds constant branches and replaces the branch with literals.</summary>
        /// <param name="node">The node of the tree to constant optimize.</param>
        /// <param name="nodes">The formula nodes to optimize.</param>
        /// <remarks>The node to replace the given one in the parent or null to not replace.</remarks>
        public INode Perform(INode node, HashSet<INode> nodes) {
            // If this node is not part of the new nodes, just return it.
            if (!nodes.Contains(node)) return node;

            // Check if the node can be turned into a constant.
            if (node.IsConstant()) {
                IConstant con = node.ToConstant();
                if (con is not null) return con;
            }

            // If the node is not a child just return the node.
            if (node is not IChild child) return node;

            // Check each parent in the child node.
            foreach (IParent parent in child.Parents.ToList()) {
                INode newNode = this.Perform(parent, nodes);
                if (newNode is not null and IParent newParent)
                    child.ReplaceParent(parent, newParent);
            }

            return node;
        }
    }
}
