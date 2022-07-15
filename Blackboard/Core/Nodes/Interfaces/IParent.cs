using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>This parent can have one or more children added and removed from it.</summary>
    /// <remarks>
    /// Since parents are nodes which can be used as a parent to a child as an input
    /// there is not limit to the number of children which may use the parent as input.
    /// That is why it allows children to be added and removed at will.
    /// Any child known by the parent is automatically updated when the parent is changed.
    /// There may be children which listen to a parent 
    /// </remarks>
    public interface IParent: INode {

        /// <summary>The set of children nodes to this node.</summary>
        public IEnumerable<IChild> Children { get; }

        /// <summary>Checks if this parent knows about the given child.</summary>
        /// <param name="child">The child to check for.</param>
        /// <returns>True if the parent has the child, false otherwise.</returns>
        public bool HasChild(IChild child);

        /// <summary>Adds children nodes onto this node.</summary>
        /// <remarks>
        /// The parent will only keep a single copy of any child
        /// even if the child uses the same parent for multiple inputs.
        /// </remarks>
        /// <param name="children">The children to add.</param>
        /// <returns>True if any children were added, false otherwise.</returns>
        public bool AddChildren(IEnumerable<IChild> children);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        /// <returns>True if any children were removed, false otherwise.</returns>
        public bool RemoveChildren(IEnumerable<IChild> children);
    }
}
