using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Parser {

    /// <summary>This is a receivable wrapper for a node which already exists.</summary>
    sealed internal class RealNode: IWrappedNode {

        /// <summary>These are any real or virtual nodes which are childern to this node.</summary>
        private Dictionary<string, IWrappedNode> children;

        /// <summary></summary>
        /// <param name="node"></param>
        public RealNode(INode node) {
            this.Node = node;
            this.children = new Dictionary<string, IWrappedNode>();
        }

        /// <summary>The type of this node.</summary>
        public System.Type Type => this.Node.GetType();

        /// <summary>Indicates if this node is currently pending or already has a real node.</summary>
        public bool Virtual => false;

        /// <summary>Gets the real node.</summary>
        public INode Node { get; private set; }

        /// <summary>Indicates if this node is an IFieldReader.</summary>
        public bool FieldReader => this.Node is IFieldReader;

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
        public bool FieldWriter => this.Node is IFieldWriter;

        /// <summary>Tries to write the given node to this node.</summary>
        /// <remarks>This node may no longer be virtual.</remarks>
        /// <param name="name">The name to write to.</param>
        /// <param name="node">The node to write</param>
        public void WriteField(string name, IWrappedNode node) {
            if (this.Node is IFieldWriter receiver) {
                INode n = node.Node;
                if (n is null) this.children[name] = node;
                else receiver.WriteField(name, n);
            }
        }
    }
}
