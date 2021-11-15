using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for all nodes in the blackboard.</summary>
    public interface INode {

        /// <summary>This will enumerate the given nodes which are not null.</summary>
        /// <remarks>This is typically used for preparing parents and children lists.</remarks>
        /// <param name="values">The nodes to enumerate.</param>
        /// <returns>The enumerator for the passed in nodes.</returns>
        static protected IEnumerable<INode> NotNull(params INode[] values) => values.NotNull();

        /// <summary>This is the type name of the node without any type parameters.</summary>
        string TypeName { get; }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        IEnumerable<INode> Parents { get; }

        /// <summary>The set of children nodes to this node in the graph.</summary>
        IEnumerable<INode> Children { get; }
    }
}
