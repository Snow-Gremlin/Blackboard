using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Parser {

    /// <summary>
    /// This virtual node is a placeholder for a node which hasn't been created yet.
    /// A define or assignment will be performed and will assign the real node for this virtual node.
    /// Other pending performers can reference the virtual node.
    /// </summary>
    sealed public class VirtualNode {

        /// <summary>The name of this virtual node.</summary>
        public readonly string Name;

        /// <summary>The tye of this virtual node.</summary>
        public readonly S.Type Type;

        /// <summary>This is the real receiver for this virtual node.</summary>
        public readonly INode Receiver;

        /// <summary>This is the virtual receiver for this virtual node.</summary>
        /// <remarks>This must be null if the real receiver is not-null.</remarks>
        public readonly VirtualNode VirtualReceiver;

        /// <summary>
        /// This is the real node this virtual node was for
        /// or null if the real node was not created yet.
        /// </summary>
        private INode node;

        /// <summary>Created a new virtual node.</summary>
        /// <param name="name">The name of the virtual node.</param>
        /// <param name="type">The type of the virtual node.</param>
        /// <param name="receiver">The real receiver for this virtual node.</param>
        public VirtualNode(string name, S.Type type, INode receiver) {
            this.Name = name;
            this.Type = type;
            this.Receiver = receiver;
            this.VirtualReceiver = null;
            this.node = null;
        }

        /// <summary>Created a new virtual node.</summary>
        /// <param name="name">The name of the virtual node.</param>
        /// <param name="type">The type of the virtual node.</param>
        /// <param name="receiver">The virtual receiver for this virtual node.</param>
        public VirtualNode(string name, S.Type type, VirtualNode receiver) {
            this.Name = name;
            this.Type = type;
            this.Receiver = null;
            this.VirtualReceiver = receiver;
            this.node = null;
        }

        /// <summary>Gets or sets the real node.</summary>
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
