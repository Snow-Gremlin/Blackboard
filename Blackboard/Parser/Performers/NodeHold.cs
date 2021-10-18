using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Parser.Performers {

    /// <summary>This will hold a node which already exists.</summary>
    /// <remarks>This must NOT hold a node which has been wrapped or a node which may contain a wrapped node.</remarks>
    sealed internal class NodeHold: IPerformer {

        /// <summary>The node to return when performed.</summary>
        public readonly INode Node;

        /// <summary>Creates a new node holder.</summary>
        /// <param name="node">The node being held.</param>
        public NodeHold(INode node) {
            this.Node = node;
        }

        /// <summary>Gets the type of the node which will be returned.</summary>
        public S.Type Type => this.Node.GetType();

        /// <summary>This will perform the actions that need to be run.</summary>
        /// <remarks>
        /// This should not throw an exception if prepared correctly.
        /// If this does throw an exception the preppers should be fixed to prevent this.
        /// </remarks>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform() => this.Node;
    }
}
