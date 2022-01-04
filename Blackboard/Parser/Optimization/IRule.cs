using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Parser.Optimization {

    /// <summary>The interface for an optimization rule.</summary>
    internal interface IRule {

        /// <summary>Performs optimization on the given nodes and surrounding nodes.</summary>
        /// <param name="root">The root node of the tree to optimize.</param>
        /// <param name="nodes">The new nodes for a formula which need to be optimized.</param>
        /// <remarks>The node to replace the given root with or the given root.</remarks>
        public INode Perform(INode root, HashSet<INode> nodes);
    }
}
