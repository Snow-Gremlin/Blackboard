using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for all nodes in the blackboard.</summary>
    public interface INode {

        /// <summary>The depth in the graph from the furthest input of this node.</summary>
        int Depth { get; }

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>
        /// The set of children that should be updated based on the results of this update.
        /// If this evaluation made no change then typically no children will be returned.
        /// Usually the entire set of children are returned on change, but it is not required.
        /// </returns>
        IEnumerable<INode> Eval();

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        IEnumerable<INode> Parents { get; }

        /// <summary>The set of children nodes to this node in the graph.</summary>
        IEnumerable<INode> Children { get; }

        /// <summary>Adds children nodes onto this node.</summary>
        /// <remarks>This will always check for loops.</remarks>
        /// <param name="children">The children to add.</param>
        void AddChildren(params INode[] children);

        /// <summary>Adds children nodes onto this node.</summary>
        /// <param name="children">The children to add.</param>
        /// <param name="checkedForLoops">Indicates if loops in the graph should be checked for.</param>
        void AddChildren(IEnumerable<INode> children, bool checkedForLoops = true);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        void RemoveChildren(params INode[] children);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        void RemoveChildren(IEnumerable<INode> children);
    }
}
