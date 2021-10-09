using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Performers {

    /// <summary>
    /// This is a node being held so it can be returned unchanged during perform.
    /// This should only be used for existing nodes or literals.
    /// This should NOT be used for nodes which are virtual or may have virtual node children.
    /// </summary>
    sealed internal class NodeHold: IPerformer {

        /// <summary>The real node being held onto.</summary>
        public INode Node;

        /// <summary>Create a new node holder.</summary>
        /// <param name="location">The location the node was gotten/created.</param>
        /// <param name="node">The node being held.</param>
        public NodeHold(Location location, INode node) {
            this.Location = location;
            this.Node = node;
        }

        /// <summary>The location this node was gotten/created.</summary>
        public Location Location { get; private set; }

        /// <summary>The type of the node.</summary>
        /// <returns>The node type.</returns>
        public System.Type ReturnType => this.Node.GetType();

        /// <summary>This will return the node being held.</summary>
        /// <param name="formula">Not used.</param>
        /// <returns>This the real node being held.</returns>
        public INode Perform(Formula formula) => this.Node;
    }
}
