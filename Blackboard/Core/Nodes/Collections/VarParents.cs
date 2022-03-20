using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections {

    /// <summary>
    /// This is a parent collection for a child which has a
    /// variable number of parents (nary nodes).
    /// </summary>
    internal class VarParents<T>: IParentCollection
            where T : class, IParent {

        private readonly List<T> source;

        /// <summary>Creates a variable parent collection.</summary>
        /// <param name="child">The child this parent collection is for.</param>
        /// <param name="source">The list from the child containing the parents.</param>
        public VarParents(IChild child, List<T> source) {
            this.Child = child;
            this.source = source;
        }

        /// <summary>The child that this parent collection is from.</summary>
        public IChild Child { get; }

        /// <summary>The set of type that each parent could have in this collection.</summary>
        /// <remarks>For Nary nodes this can be very long so use this with zip or limit the enumerations.</remarks>
        public IEnumerable<S.Type> Types => new S.Type[1] { typeof(T) }.RepeatLast();

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
        /// </remarks>
        /// <returns>An enumerator for all the parents.</returns>
        public IEnumerator<IParent> GetEnumerator() => this.source.NotNull().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>Indicates that this collection is variable (not fixed).</summary>
        public bool Fixed => false;

        /// <summary>This is the number of parents in the collection.</summary>
        public int Count => this.source.Count;

        /// <summary>This gets or sets the parent at the given location.</summary>
        /// <remarks>This will throw an exception if out-of-bounds or wrong type.</remarks>
        /// <param name="index">The index to get or set from.</param>
        /// <returns>The parent gotten from the given index.</returns>
        public IParent this[int index] {
            get => this.source[index];
            set {
                if (ReferenceEquals(this.source[index], value)) return;

                if (value is not T newParent)
                    throw new Message("Incorrect type of a parent being set to a node.").
                        With("child", this.Child).
                        With("index", index).
                        With("expected type", typeof(T)).
                        With("gotten type", value.GetType());

                bool removed = this.source[index]?.RemoveChildren(this.Child) ?? false;
                this.source[index] = newParent;
                if (removed) newParent?.AddChildren(this.Child);
            }
        }

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <remarks>
        /// The new parent must be able to take the place of the old parent,
        /// otherwise this will throw an exception when attempting the replacement of the old parent.
        /// </remarks>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool Replace(IParent oldParent, IParent newParent) {
            bool changed = false;
            bool typeChecked = false;
            bool removed = false;
            for (int i = this.source.Count - 1; i >= 0; --i) {
                T node = this.source[i];
                if (!ReferenceEquals(node, oldParent)) continue;

                // Now that at least one parent will be replaced, check that the new parent can be used.
                if (!typeChecked && newParent is not null and not T)
                    throw new Message("Unable to replace old parent with new parent in a list.").
                        With("child", this.Child).
                        With("index", i).
                        With("node", node).
                        With("old parent", oldParent).
                        With("new parent", newParent).
                        With("target type", typeof(T));
                typeChecked = true;

                // Replace parent in list of sources.
                if (ReferenceEquals(node, newParent)) continue;
                removed = node?.RemoveChildren(this.Child) ?? false;
                this.source[i] = newParent as T;
                changed = true;
            }
            if (removed) newParent?.AddChildren(this.Child);
            return changed;
        }

        /// <summary>This will attempt to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAll(List<IParent> newParents) {
            int index = 0;
            foreach (IParent parent in newParents) {
                if (parent is not T)
                    throw new Message("Incorrect type of a parent in the list of parents to set to a node.").
                        With("child", this.Child).
                        With("index", index).
                        With("expected type", typeof(T)).
                        With("gotten type", parent.GetType());
                ++index;
            }

            int oldCount = this.source.Count;
            int newCount = newParents.Count;
            int minCount = S.Math.Min(oldCount, newCount);

            bool changed = oldCount != newCount;
            for (int i = 0; i < minCount; ++i) {
                if (!ReferenceEquals(this.source[i], newParents[i])) {
                    bool removed = this.source[i]?.RemoveChildren(this.Child) ?? false;
                    if (removed) newParents[i]?.AddChildren(this.Child);
                    changed = true;
                }
            }

            for (int i = minCount; i < oldCount; ++i)
                this.source[i]?.RemoveChildren(this.Child);
            this.source.Clear();
            this.source.AddRange(newParents.OfType<T>());
            return changed;
        }

        /// <summary>This inserts new parents into the given location.</summary>
        /// <param name="index">The index to insert the new parents into.</param>
        /// <param name="newParents">The set of new parents to insert.</param>
        /// <param name="oldChild">
        /// If the parents were being moved from one node onto another node,
        /// then this is the old child to remove when applying the parents to this node.
        /// If the old child has been removed from the parent, then this child is added to the parent.
        /// If there is no old child then this child is not set to the new parents.
        /// </param>
        /// <returns>True if any parents were added, false otherwise.</returns>
        public bool Insert(int index, IEnumerable<IParent> newParents, IChild oldChild = null) {
            if (index < 0 || index > this.source.Count)
                throw new Message("Invalid index to insert parents at.").
                    With("child", this.Child).
                    With("count", this.source.Count).
                    With("index", index);

            foreach ((IParent parent, int i) in newParents.WithIndex()) {
                if (parent is not T)
                    throw new Message("Incorrect type of a parent in the list of parents to insert into a node.").
                        With("child", this.Child).
                        With("insert index", index).
                        With("parent index", i).
                        With("expected type", typeof(T)).
                        With("gotten type", parent.GetType());
            }

            bool changed = false;
            foreach (IParent parent in newParents) {
                bool removed = oldChild is not null && (parent?.RemoveChildren(oldChild) ?? false);
                if (removed) parent?.AddChildren(this.Child);
                changed = true;
            }

            this.source.InsertRange(index, newParents.NotNull().OfType<T>());
            return changed;
        }

        /// <summary>This removes one or more parent at the given location.</summary>
        /// <param name="index">The index to start removing the parents from.</param>
        /// <param name="length">The number of parents to remove.</param>
        public void Remove(int index, int length = 1) {
            if (index < 0 || length < 0 || index+length >  this.source.Count)
                throw new Message("Not a valid range of parents to remove.").
                    With("child", this.Child).
                    With("count", this.source.Count).
                    With("index", index).
                    With("length", length);

            for (int i = index, j = 0; j < length; ++i, ++j)
                this.source[i]?.RemoveChildren(this.Child);
            this.source.RemoveRange(index, length);
        }

        /// <summary>This creates a string for the given set of parents.</summary>
        /// <returns>The string for this set of parents.</returns>
        public override string ToString() => "[" + this.source.Join(", ") + "]";
    }
}
