using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>This parent can have one or more children added and removed from it.</summary>
    /// <remarks>
    /// Since parents are nodes which can be used as a parent to a child as an input
    /// there is not limit to the number of children which may use the parent as input.
    /// That is why it allowes children to be added and removed at will.
    /// Any child known by the parent is automatically updated when the parent is changed.
    /// There may be children which listen to a parent 
    /// </remarks>
    public interface IParent: INode {

        /// <summary>The set of children nodes to this node.</summary>
        public IEnumerable<IChild> Children { get; }

        /// <summary>Adds children nodes onto this node.</summary>
        /// <param name="children">The children to add.</param>
        public void AddChildren(params IChild[] children) =>
            this.AddChildren(children as IEnumerable<IChild>);

        /// <summary>Adds children nodes onto this node.</summary>
        /// <param name="children">The children to add.</param>
        public void AddChildren(IEnumerable<IChild> children);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(params IChild[] children) =>
            this.RemoveChildren(children as IEnumerable<IChild>);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(IEnumerable<IChild> children);
    }
}
