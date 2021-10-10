using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Prepers {

    /// <summary>This is for storing a node which needs to be returned directly in the performer.</summary>
    sealed internal class NoPrep: IPreper {

        /// <summary>Creates a new non-preper preper.</summary>
        /// <param name="loc">The location this node was looked up at.</param>
        /// <param name="node">The node that should be passed through.</param>
        public NoPrep(Location loc, INode node) {
            this.NodeHold = new NodeHold(loc, node);
        }

        /// <summary>Creates a new non-preper preper.</summary>
        /// <param name="hold">The node holder to return when prepared.</param>
        public NoPrep(NodeHold hold) {
            this.NodeHold = hold;
        }

        /// <summary>The node holder to return when prepared.</summary>
        public NodeHold NodeHold;

        /// <summary>The location from the node holder.</summary>
        public Location Location => this.NodeHold.Location;

        /// <summary>Does no preperation and just returns the held node.</summary>
        /// <param name="formula">Not Used.</param>
        /// <param name="option">Not Used.</param>
        /// <returns>The node being held.</returns>
        public IPerformer Prepare(Formula formula, Options option) => this.NodeHold;
    }
}
