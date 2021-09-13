using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>This means the nodes can have children added and removed from it.</summary>
    public interface IAdopter {

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
