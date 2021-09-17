using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Parser.Actors.Caps {

    /// <summary>An actor which contains an existing node.</summary>
    sealed internal class Node: IActor {

        /// <summary>Creates a new node actor.</summary>
        /// <param name="node">The existing node to store.</param>
        public Node(INode node) {
            this.Existing = node;
        }

        /// <summary>The existing node being stored.</summary>
        public INode Existing;

        /// <summary>Reduces this actor to a literal of the value without writing any nodes.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode Evaluate() => this.Existing;

        /// <summary>Creates and writes a node to Blackboard.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode BuildNode() => this.Existing;

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public Type Returns() => Type.TypeOf(this.Existing);
    }
}
