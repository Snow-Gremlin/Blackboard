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
                // These exceptions should never be hit if the preppers is working as expected.
                if (!value.GetType().IsAssignableTo(this.Type))
                    throw new Exception("The virtual node resolved an uexpected type of node.").
                        With("Name", this.Name).
                        With("Type", this.Type).
                        With("Node", value.GetType());
                
                if (this.Receiver.Virtual)
                    throw new Exception("The receiver for a virtual node must be resolved prior to the child node.").
                        With("Receiver", this.Receiver).
                        With("Name", this.Name).
                        With("Type", this.Type);

                if (!this.Receiver.IsFieldReader || !this.Receiver.IsFieldWriter)
                    throw new Exception("The receiver for a virtual node must be a field reader/writer.").
                        With("Receiver", this.Receiver).
                        With("Name", this.Name).
                        With("Type", this.Type);

                IWrappedNode child = this.Receiver.ReadField(this.Name);
                if (child is null)
                    throw new Exception("The receiver does not contain a child by the virtual node's name when being resolved.").
                        With("Name", this.Name).
                        With("Receiver", this.Receiver).
                        With("Node", value.GetType());

                if (!ReferenceEquals(child, this))
                    throw new Exception("The virtual node was being resolved with a node which does not match the receiver's child of that name.").
                        With("Name", this.Name).
                        With("Receiver", this.Receiver).
                        With("Child", child.GetType()).
                        With("Node", value.GetType());

                this.node = value;
                IFieldWriter receiver = this.Receiver.Node as IFieldWriter;
                receiver.WriteField(this.Name, value);
            }
        }

        /// <summary>Indicates if this node is an IFieldReader.</summary>
        public bool IsFieldReader => this.Type.IsAssignableTo(typeof(IFieldReader));

        /// <summary>Reads a node from the field reader being represented.</summary>
        /// <param name="name">The name of the node to look up.</param>
        /// <returns>The node read from this field reader or null.</returns>
        public IWrappedNode ReadField(string name) {
            if (this.IsFieldReader) {
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
        public bool IsFieldWriter => this.Type.IsAssignableTo(typeof(IFieldWriter));

        /// <summary>Tries to create a new node and add it to the node.</summary>
        /// <param name="name">The name of the field to add.</param>
        /// <param name="type">The type of the field to add.</param>
        /// <returns>The new virtual node for this node.</returns>
        public VirtualNode CreateField(string name, S.Type type) {
            if (!this.IsFieldWriter)
                throw new Exception("May not write a field to a node which is not a field writer").
                    With("Receiver", this.Node).
                    With("Name", name).
                    With("Type", type);

            if (this.ReadField(name) is not null)
                throw new Exception("A field by that name already exists in the field writer").
                    With("Receiver", this.Node).
                    With("Name", name).
                    With("Type", type);

            VirtualNode node = new(name, type, this);
            this.children[name] = node;
            return node;
        }

        /// <summary>Gets the virtual node debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() {
            const string indent = "  ";
            string tail = this.children.Count <= 0 ? "" :
                "[\n" + indent + string.Join("\n" + indent,
                    this.children.SelectFromPairs((string name, IWrappedNode node) => name + ": " + node)
                ) + "\n]";
            return "VirtualNode("+(this.Virtual ? this.Type : this.Node)+")"+tail;
        }
    }
}
