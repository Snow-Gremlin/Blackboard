using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Parser {

    /// <summary>
    /// This node is a placeholder for a real node or a virtual node which hasn't been created yet.
    /// A define or assignment will be performed and will assign the real node for a virtual node.
    /// Other pending performers can reference the real or virtual node.
    /// </summary>
    sealed public class NodeHold {

        /// <summary>The name of this node.</summary>
        /// <remarks>This may be empty if a real node.</remarks>
        public readonly string Name;

        /// <summary>The tye of this node.</summary>
        public readonly S.Type Type;

        /// <summary>This is the receiver for this node.</summary>
        /// <remarks>This may be null if the node is real.</remarks>
        public readonly NodeHold Receiver;

        /// <summary>This is the real node or null if still a virtual node.</summary>
        private INode node;

        /// <summary>Created a new virtual node.</summary>
        /// <param name="name">The name of the virtual node.</param>
        /// <param name="type">The type of the virtual node.</param>
        /// <param name="receiver">The receiver for this virtual node.</param>
        public NodeHold(string name, S.Type type, NodeHold receiver) {
            this.Name = name;
            this.Type = type;
            this.Receiver = receiver;
            this.node = null;
        }

        /// <summary>Created a new real node.</summary>
        /// <param name="node">The real node.</param>
        /// <param name="name">The optional name of the real node.</param>
        public NodeHold(INode node, string name = "") {
            this.Name = name;
            this.Type = node.GetType();
            this.Receiver = null;
            this.node = node;
        }

        /// <summary>Indicates if this node is currently pending or already has a real node.</summary>
        public bool Virtual => this.node is null;

        /// <summary>Gets or sets the real node. If virtual this will return null.</summary>
        /// <remarks>This must be set by the define or assign and must be the correct type.</remarks>
        public INode Node {
            get => this.node;
            set {
                if (value.GetType() != this.Type)
                    // This expection should never be hit if the prepers is working as expected.
                    throw new Exception("The virtual node resolved an uexpected type of node.").
                        With("Name", this.Name).
                        With("Type", this.Type).
                        With("Node", value.GetType());
                this.node = value;
            }
        }
    }
}
