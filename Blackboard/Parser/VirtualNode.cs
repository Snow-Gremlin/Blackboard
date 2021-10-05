using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Parser {

    /// <summary>
    /// This node is a placeholder for a virtual node which hasn't been created yet.
    /// A define or assignment will be performed and will assign the real node for a virtual node.
    /// Other pending performers can reference the virtual node.
    /// </summary>
    sealed internal class VirtualNode: IWrappedNode {

        /// <summary>The name of this node.</summary>
        public readonly string Name;

        /// <summary>This is the receiver for this node.</summary>
        public readonly IWrappedNode Receiver;

        /// <summary>This is the real node or null if still a virtual node.</summary>
        private INode node;

        /// <summary>These are any real or virtual nodes which are childern to this node.</summary>
        private Dictionary<string, IWrappedNode> children;

        /// <summary>Created a new virtual node.</summary>
        /// <param name="name">The name of the virtual node.</param>
        /// <param name="type">The type of the virtual node.</param>
        /// <param name="receiver">The receiver for this virtual node.</param>
        public VirtualNode(string name, S.Type type, IWrappedNode receiver) {
            if (receiver is null)
                throw new Exception("A non-null receiver is required for a virtual node.").
                     With("Name", this.Name).
                     With("Type", this.Type);

            this.Name = name;
            this.Type = type;
            this.Receiver = receiver;
            this.node = null;
            this.children = new Dictionary<string, IWrappedNode>();
        }

        /// <summary>The type of this node.</summary>
        public S.Type Type { get; private set; }

        /// <summary>Indicates if this node is currently pending or already has a real node.</summary>
        public bool Virtual => this.node is null;

        /// <summary>Gets or sets the real node. If virtual this will return null.</summary>
        /// <remarks>This must be set by the define or assign and must be the correct type.</remarks>
        public INode Node {
            get => this.node;
            set {
                if (value.GetType() != this.Type)
                    // This exception should never be hit if the prepers is working as expected.
                    throw new Exception("The virtual node resolved an uexpected type of node.").
                        With("Name", this.Name).
                        With("Type", this.Type).
                        With("Node", value.GetType());
                
                // Check that receiver has been resolved.
                if (this.Receiver.Virtual)
                    throw new Exception("The receiver for a virtual node must be resolved prior to the child node.").
                        With("Receiver", this.Receiver.ToString()).
                        With("Name", this.Name).
                        With("Type", this.Type);

                IWrappedNode child = this.Receiver.ReadField(this.Name);
                if (child is null)
                    throw new Exception("The receiver does not contain a child by the virtual node's name when being resolved.").
                        With("Name", this.Name).
                        With("Receiver", this.Receiver.ToString()).
                        With("Node", value.GetType());

                if (!ReferenceEquals(child, value))
                    throw new Exception("The virtual node was being resolved with a node which does not match the receiver's child of that name.").
                        With("Name", this.Name).
                        With("Receiver", this.Receiver.ToString()).
                        With("Child", child.GetType()).
                        With("Node", value.GetType());

                this.node = value;
            }
        }

        /// <summary>Indicates if this node is an IFieldReader.</summary>
        public bool FieldReader => this.Type.IsAssignableTo(typeof(IFieldReader));

        /// <summary>Reads a node from the field reader being represented.</summary>
        /// <param name="name">The name of the node to look up.</param>
        /// <returns>The node read from this field reader or null.</returns>
        public IWrappedNode ReadField(string name) {
            if (this.FieldReader) {
                if (this.children.TryGetValue(name, out IWrappedNode child))
                    return child;

                if (!this.Virtual && this.Node is IFieldReader receiver) {
                    INode childNode = receiver.ReadField(name);
                    if (childNode is not null) {
                        child = new RealNode(childNode);
                        this.children[name] = child;
                        return child;
                    }
                }
            }
            return null;
        }

        /// <summary>Indicates if this node is an IFieldWriter.</summary>
        public bool FieldWriter => this.Type.IsAssignableTo(typeof(IFieldWriter));

        /// <summary>Writes the given node to this node.</summary>
        /// <remarks>This node may no longer be virtual.</remarks>
        /// <param name="name">The name to write to.</param>
        /// <param name="node">The node to write</param>
        public void WriteField(string name, IWrappedNode node) {

            // TODO: Finish

        }
    }
}
