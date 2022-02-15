using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Collections {

    /// <summary>This is a collection of parent nodes for an IChild.</summary>
    public interface IParentCollection {

        /// <summary>The child that this parent collection is from.</summary>
        public IChild Child { get; }
        
        /// <summary>The set of type that each parent could have in this collection.</summary>
        /// <remarks>For N-ary nodes this can be very long so use this with zip or limit the enumerations.</remarks>
        public IEnumerable<S.Type> Types { get; }

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
        /// Since null parents are removed, this list may not be the same as the Count even if fixed.
        /// </remarks>
        public IEnumerable<IParent> Nodes { get; }

        /// <summary>
        /// Fixed indicates the number of parents may not change,
        /// otherwise the parents may be variable length.
        /// </summary>
        public bool Fixed { get; }

        /// <summary>
        /// The number of parents currently in the collection or the fixed length of parents.
        /// </summary>
        public int Count { get; }

        /// <summary>This gets or sets the parent at the given location.</summary>
        /// <remarks>This will throw an exception if out-of-bounds or wrong type.</remarks>
        /// <param name="index">The index to get or set from.</param>
        /// <returns>The parent gotten from the given index.</returns>
        public IParent this[int index] { get; set; }

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <remarks>
        /// The new parent must be able to take the place of the old parent,
        /// otherwise this will throw an exception when attempting the replacement of the old parent.
        /// </remarks>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool Replace(IParent oldParent, IParent newParent);

        /// <summary>This attempts to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAll(List<IParent> newParents);

        /// <summary>This inserts new parents into the given location.</summary>
        /// <remarks>This will throw an exception for fixed parent collections.</remarks>
        /// <param name="index">The index to insert the new parents into.</param>
        /// <param name="newParents">The set of new parents to insert.</param>
        /// <param name="oldChild">
        /// If the parents were being moved from one node onto another node,
        /// then this is the old child to remove when applying the parents to this node.
        /// If the old child has been removed from the parent, then this child is added to the parent.
        /// If there is no old child then this child is not set to the new parents.
        /// </param>
        /// <returns>True if any parents were added, false otherwise.</returns>
        public bool Insert(int index, IEnumerable<IParent> newParents, IChild oldChild = null);

        /// <summary>This remove one or more parent at the given location.</summary>
        /// <remarks>This will throw an exception for fixed parent collections.</remarks>>
        /// <param name="index">The index to start removing the parents from.</param>
        /// <param name="length">The number of parents to remove.</param>
        public void Remove(int index, int length = 1);
    }
}