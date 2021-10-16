using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;

namespace Blackboard.Parser.Prepers {

    /// <summary>This is for storing a node which needs to be returned directly in the performer.</summary>
    sealed internal class NoPrep: IPreper {

        /// <summary>Creates a new non-preper preper.</summary>
        /// <param name="node">The node that should be passed through.</param>
        public NoPrep(INode node) {
            this.NodeHold = new NodeHold(node);
        }

        /// <summary>Creates a new non-preper preper.</summary>
        /// <param name="hold">The node holder to return when prepared.</param>
        public NoPrep(NodeHold hold) {
            this.NodeHold = hold;
        }

        /// <summary>The node holder to return when prepared.</summary>
        public NodeHold NodeHold;

        /// <summary>Does no preperation and just returns the held node.</summary>
        /// <param name="formula">Not Used.</param>
        /// <param name="evaluate">Not Used.</param>
        /// <returns>The node being held.</returns>
        public IPerformer Prepare(Formula formula, bool evaluate = false) => this.NodeHold;
    }
}
