using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Parser {

    /// <summary>
    /// This is the interface for a container for working with either a real or a virtual node.
    /// Since virtual nodes have to be named in order for other lines to access them this
    /// containes methods for looking up named children as if this is a receiver eventhough it could
    /// also be a leaf, such as an input or output node.
    /// </summary>
    internal interface IWrappedNode {

        /// <summary>The type of this node.</summary>
        public System.Type Type { get; }

        /// <summary>Indicates if this node is currently pending or already has a real node.</summary>
        public bool Virtual { get; }

        /// <summary>Gets the real node. If virtual this will return null.</summary>
        public INode Node { get; }

        /// <summary>Indicates if this node is an IFieldReader.</summary>
        public bool FieldReader { get; }

        /// <summary>Reads a node from the field reader being represented.</summary>
        /// <param name="name">The name of the node to look up.</param>
        /// <returns>
        /// The node read from this field reader.
        /// Returns null if doesn't exist or this isn't a field reader.
        /// </returns>
        public IWrappedNode ReadField(string name);

        /// <summary>Indicates if this node is an IFieldWriter.</summary>
        public bool FieldWriter { get; }

        /// <summary>Tries to write the given node to this node.</summary>
        /// <remarks>This node may be virtual or nots.</remarks>
        /// <param name="name">The name to write to.</param>
        /// <param name="node">The node to write</param>
        public void WriteField(string name, IWrappedNode node);
    }
}
