using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>A child node is a node which may specify many parents of the same type.</summary>
    /// <typeparam name="T">The type of parent used for this child.</typeparam>
    public interface INaryChild<T>: IChild
        where T : IParent {

        /// <summary>This adds parents to this node.</summary>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(IEnumerable<T> parents);

        /// <summary>This removes the given parents from this node.</summary>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(IEnumerable<T> parents);

        /// <summary>This removes all the parents from this node.</summary>
        public void ClearParents();
    }
}
