using Blackboard.Core.Nodes.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections;

partial class ParentCollection {

    /// <summary>This is a variable parent container.</summary>
    /// <typeparam name="T">The type of the parent that is being set.</typeparam>
    private class VarParent<T> : IVarParent
            where T : class, IParent {
        private readonly List<T> source;

        /// <summary>Creates a new variable parent container using the given list.</summary>
        /// <param name="source">The list of parents to read from and modify.</param>
        /// <param name="min">The optional inclusive minimum allowed number of variable parents.</param>
        /// <param name="max">The optional inclusive maximum allowed number of variable parents.</param>
        public VarParent(List<T> source, int min = 0, int max = int.MaxValue) {
            this.source = source;
            this.Type = typeof(T);
            if (min < 0 || max < min)
                throw new BlackboardException("Invalid minimum and maximum range for variable parents collection.").
                    With("min", min).
                    With("max", max);
            this.Minimum = min;
            this.Maximum = max;
        }

        /// <summary>Gets the type of the parents.</summary>
        public S.Type Type { get; }

        /// <summary>The inclusive minimum allowed number of variable parents.</summary>
        public int Minimum { get; }

        /// <summary>The inclusive maximum allowed number of variable parents.</summary>
        public int Maximum { get; }

        /// <summary>This gets the number of variable parents.</summary>
        public int Count => this.source.Count;

        /// <summary>This gets or sets the parent at the given index.</summary>
        /// <remarks>When setting the parent, the parent must be the type of this collection.</remarks>
        /// <param name="index">The index for the parent to get or set.</param>
        /// <returns>The parent at the given index.</returns>
        public IParent this[int index] {
            get => this.source[index];
            set => this.source[index] = value as T ??
                throw new BlackboardException("Given parent can not be cast into variable parent type.").
                    With("index", index).
                    With("type", typeof(T)).
                    With("parent", value);
        }

        /// <summary>This adds a new parent to the end of the variable parents.</summary>
        /// <remarks>
        /// This will NOT check if this will make the list go above the maximum value.
        /// When setting the parent, the parent must be the type of this collection.
        /// </remarks>
        /// <param name="item">This is the parent to add.</param>
        public void Add(IParent item) => this.source.Add(item as T ??
            throw new BlackboardException("Given parent can not be cast into variable parent type.").
                With("type", typeof(T)).
                With("parent", item));

        /// <summary>This inserts parents into the list at the given index.</summary>
        /// <remarks>
        /// This will NOT check if this will make the list go above the maximum value.
        /// When setting the parent, the parent must be the type of this collection.
        /// </remarks>
        /// <param name="index">The index to insert the parents at.</param>
        /// <param name="parents">The parent to insert into the list.</param>
        public void Insert(int index, IEnumerable<IParent> parents) => this.source.InsertRange(index, parents.OfType<T>());

        /// <summary>This removes parents starting from the given index.</summary>
        /// <remarks>This will NOT check if this will make the list go below the minimum value.</remarks>
        /// <param name="index">The index of the parents to remove.</param>
        /// <param name="count">The number of parents to remove.</param>
        public void Remove(int index, int count = 1) => this.source.RemoveRange(index, count);

        /// <summary>Gets the enumerator of the variable parents.</summary>
        /// <returns>The enumerator of the variable parents.</returns>
        public IEnumerator<IParent> GetEnumerator() => this.source.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
