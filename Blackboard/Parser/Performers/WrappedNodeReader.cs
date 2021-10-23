using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Parser.Performers {

    /// <summary>Will get the node from the given wrapped node.</summary>
    sealed internal class WrappedNodeReader: IPerformer {

        /// <summary>The reference to the real or virtual node.<</summary>
        public readonly IWrappedNode WrappedNode;

        /// <summary>Creates a new virtual node reference.</summary>
        /// <param name="wrappedNode">The reference to the real or virtual node.</param>
        public WrappedNodeReader(IWrappedNode wrappedNode) {
            this.WrappedNode = wrappedNode;
        }

        /// <summary>Gets the type of the node which will be returned.</summary>
        public S.Type Type => this.WrappedNode.Type;

        /// <summary>This will perform the actions that need to be run.</summary>
        /// <remarks>
        /// This should not throw an exception if prepared correctly.
        /// If this does throw an exception the preppers should be fixed to prevent this.
        /// </remarks>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform() => this.WrappedNode.Node;

        /// <summary>Gets the performer debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() => "WrappedNodeReader("+this.WrappedNode+")";
    }
}
