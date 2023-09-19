using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Collections;

partial class ParentCollection {

    /// <summary>The definition for all variable parent container.</summary>
    private interface IVarParent : IEnumerable<IParent> {

        /// <summary>Gets the type of the container.</summary>
        public S.Type Type { get; }

        /// <summary>The inclusive minimum allowed number of variable parents.</summary>
        public int Minimum { get; }

        /// <summary>The inclusive maximum allowed number of variable parents.</summary>
        public int Maximum { get; }

        /// <summary>This gets the number of variable parents.</summary>
        public int Count { get; }

        /// <summary>This gets or sets the parent at the given index.</summary>
        /// <remarks>When setting the parent, the parent must be the type of this collection.</remarks>
        /// <param name="index">The index for the parent to get or set.</param>
        /// <returns>The parent at the given index.</returns>
        public IParent this[int index] { get; set; }

        /// <summary>This adds a new parent to the end of the variable parents.</summary>
        /// <remarks>
        /// This will NOT check if this will make the list go above the maximum value.
        /// When setting the parent, the parent must be the type of this collection.
        /// </remarks>
        /// <param name="item">This is the parent to add.</param>
        public void Add(IParent item);

        /// <summary>This inserts parents into the list at the given index.</summary>
        /// <remarks>
        /// This will NOT check if this will make the list go above the maximum value.
        /// When setting the parent, the parent must be the type of this collection.
        /// </remarks>
        /// <param name="index">The index to insert the parents at.</param>
        /// <param name="parents">The parents to insert into the list.</param>
        public void Insert(int index, IEnumerable<IParent> parents);

        /// <summary>This removes parents starting from the given index.</summary>
        /// <remarks>This will NOT check if this will make the list go below the minimum value.</remarks>
        /// <param name="index">The index of the parents to remove.</param>
        /// <param name="count">The number of parents to remove.</param>
        public void Remove(int index, int count = 1);
    }
}
