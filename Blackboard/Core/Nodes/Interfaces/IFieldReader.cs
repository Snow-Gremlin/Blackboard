using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>This indicates that the node has fields that can be read.</summary>
    public interface IFieldReader: INode {

        /// <summary>Determines if the given field by name exists.</summary>
        /// <param name="name">The name of the field to look for.</param>
        /// <returns>True if the name exists in this node node.</returns>
        public bool ContainsField(string name);

        /// <summary>Reads the node for the field by the given name.</summary>
        /// <param name="name">The name for the node to look for.</param>
        /// <returns>The node or null if not found.</returns>
        public INode ReadField(string name);

        /// <summary>Finds the node at the given path.</summary>
        /// <param name="names">The names to the node to find.</param>
        /// <returns>The node at the end of the path or null.</returns>
        public INode Find(params string[] names);

        /// <summary>Finds the node at the given path.</summary>
        /// <param name="names">The names to the node to find.</param>
        /// <returns>The node at the end of the path or null.</returns>
        public INode Find(IEnumerable<string> names);
    }
}
