using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Parser {

    /// <summary>This is a receivable wrapper for a node which already exists.</summary>
    sealed internal class RealNode: IWrappedNode {

        /// <summary>These are any real or virtual nodes which are childern to this node.</summary>
        private Dictionary<string, IWrappedNode> children;

        /// <summary></summary>
        /// <param name="node"></param>
        public RealNode(INode node) {
            this.Node = node;
            this.children = new();
        }

        /// <summary>The type of this node.</summary>
        public S.Type Type => this.Node.GetType();

        /// <summary>Indicates if this node is currently pending or already has a real node.</summary>
        public bool Virtual => false;

        /// <summary>Gets the real node.</summary>
        public INode Node { get; private set; }

        /// <summary>Indicates if this node is an IFieldReader.</summary>
        public bool IsFieldReader => this.Node is IFieldReader;

        /// <summary>Reads a node from the field reader being represented.</summary>
        /// <param name="name">The name of the node to look up.</param>
        /// <returns>The node read from this field reader or null.</returns>
        public IWrappedNode ReadField(string name) {
            if (this.Node is IFieldReader receiver) {
                if (this.children.TryGetValue(name, out IWrappedNode child))
                    return child;

                INode childNode = receiver.ReadField(name);
                if (childNode is not null) {
                    child = new RealNode(childNode);
                    this.children[name] = child;
                    return child;
                }
            }
            return null;
        }

        /// <summary>Indicates if this node is an IFieldWriter.</summary>
        public bool IsFieldWriter => this.Node is IFieldWriter;

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

        /// <summary>Gets the real node debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() => "RealNode("+this.Node+")";
    }
}
