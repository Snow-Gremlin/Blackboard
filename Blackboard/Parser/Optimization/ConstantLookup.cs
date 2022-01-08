using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Optimization {

    /// <summary>Replace all constants with constants stored on the slate to reduce duplicates.</summary>
    sealed internal class ConstantLookup: IRule {

        /// <summary>Creates a new constant lookup rule.</summary>
        public ConstantLookup() { }

        /// <summary>Recursively finds constant branches and replaces the branch with literals.</summary>
        /// <param name="node">The node of the tree to constant optimize.</param>
        /// <param name="nodes">The formula nodes to optimize.</param>
        /// <param name="logger">The logger to debug and inspect the optimization.</param>
        /// <remarks>The node to replace the given one in the parent or null to not replace.</remarks>
        public INode Perform(INode node, HashSet<INode> nodes, ILogger logger = null) {
            

            return null;
        }
    }
}
