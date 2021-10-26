using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Parser {

    /// <summary>
    /// This is the interface for a container for working with either a real or a virtual node.
    /// </summary>
    /// <remarks>
    /// Since virtual nodes have to be named in order for other lines to access them this
    /// containes methods for looking up named children as if this is a receiver eventhough
    /// it could also be a leaf, such as an input or output node.
    /// </remarks>
    internal interface IWrappedNode {

        /// <summary>The type of this node.</summary>
        public S.Type Type { get; }

        /// <summary>Indicates if this node is currently pending or already has a real node.</summary>
        public bool Virtual { get; }

        /// <summary>Gets the real node. If virtual this will return null.</summary>
        public INode Node { get; }

        /// <summary>Indicates if this node is an IFieldReader.</summary>
        public bool IsFieldReader { get; }

        /// <summary>Reads a node from the field reader being represented.</summary>
        /// <remarks>This node must be a field reader.</remarks>
        /// <param name="name">The name of the node to look up.</param>
        /// <returns>The node read from this field reader or null if doesn't exist.</returns>
        public IWrappedNode ReadField(string name);

        /// <summary>Indicates if this node is an IFieldWriter.</summary>
        public bool IsFieldWriter { get; }

        /// <summary>Tries to create a new node and add it to the node.</summary>
        /// <remarks>This node must be a field writer and may not contain this field already.</remarks>
        /// <param name="name">The name of the field to add.</param>
        /// <param name="type">The type of the field to add.</param>
        /// <returns>The new virtual node for this node.</returns>
        public VirtualNode CreateField(string name, S.Type type);

        /// <summary>Gets the virtual node debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public string ToString();

        /// <summary>Gets the virtual node as a simple string without any children.</summary>
        /// <returns>A human readable debug string.</returns>
        public string ToSimpleString();
    }
}
