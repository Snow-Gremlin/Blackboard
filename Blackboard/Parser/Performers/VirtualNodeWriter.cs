using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Parser.Performers {

    /// <summary>Will perform the given value and write it to the given virtual node.</summary>
    sealed internal class VirtualNodeWriter: IPerformer {

        /// <summary>The Virtual node to write to.</summary>
        public readonly VirtualNode VirtualNode;

        /// <summary>The value to set.</summary>
        public readonly IPerformer Value;

        /// <summary>Creates a new virtual node writer.</summary>
        /// <param name="virtualNode">The virtual node to write to.</param>
        /// <param name="value">The performer to get the node to write.</param>
        public VirtualNodeWriter(VirtualNode virtualNode, IPerformer value) {
            this.VirtualNode = virtualNode;
            this.Value = value;
        }

        /// <summary>Gets the type of the node which will be returned.</summary>
        public S.Type Type => this.VirtualNode.Type;

        /// <summary>This will perform the actions that need to be run.</summary>
        /// <remarks>
        /// This should not throw an exception if prepared correctly.
        /// If this does throw an exception the preppers should be fixed to prevent this.
        /// </remarks>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform() {
            INode node = this.Value.Perform();
            this.VirtualNode.Node = node;
            return node;
        }
    }
}
