using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections {

    /// <summary>This is a collection of parent nodes for an IChild.</summary>
    /// <remarks>
    /// This should not contain null parents, but it might contain repeat parents.
    /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
    /// Since null parents are removed, this list may not be the same as the Count even if fixed.
    /// </remarks>
    public class ParentCollection : IEnumerable<IParent> {

        #region Parent Parameters...

        /// <summary>The definition for a single fixed parent container.</summary>
        private interface IFixedParent {

            /// <summary>Gets the type of the container.</summary>
            public S.Type Type { get; }

            /// <summary>Gets or set the parent.</summary>
            public IParent Node { get; set; }
        }

        /// <summary>This is a single fixed parent container.</summary>
        /// <typeparam name="T">The type of the parent that is being set.</typeparam>
        private class FixedParent<T> : IFixedParent
                where T : class, IParent {
            private readonly S.Func<T> getParent;
            private readonly S.Action<T> setParent;

            /// <summary>Creates a new fixed parent container using the given getter or setter.</summary>
            /// <param name="getParent">The getter to get the parent value from the child.</param>
            /// <param name="setParent">The setter to set a parent value to the child.</param>
            public FixedParent(S.Func<T> getParent, S.Action<T> setParent) {
                this.getParent = getParent;
                this.setParent = setParent;
            }

            /// <summary>Gets the type of the parent.</summary>
            public S.Type Type => typeof(T);

            /// <summary>Gets or sets the parent.</summary>
            /// <remarks>This parent must be able to case to the container type.</remarks>
            public IParent Node {
                get => this.getParent();
                set => this.setParent(value as T);
            }
        }

        /// <summary>The definition for all variable parent container.</summary>
        private interface IVarParent : IEnumerable<IParent> {

            /// <summary>Gets the type of the container.</summary>
            public S.Type Type { get; }

            /// <summary>This gets the number of variable parents.</summary>
            public int Count { get; }

            /// <summary>This gets or sets the parent at the given index.</summary>
            /// <param name="index">The index for the parent to get or set.</param>
            /// <returns>The parent at the given index.</returns>
            public IParent this[int index] { get; set; }

            /// <summary>This adds a new parent to the end of the variable parents.</summary>
            /// <param name="item">This is the parent to add.</param>
            public void Add(IParent item);

            /// <summary>This inserts a parent into the list at the given index.</summary>
            /// <param name="index">The index to insert the parent at.</param>
            /// <param name="parent">The parent to insert into the list.</param>
            public void Insert(int index, IParent parent);

            /// <summary>This removes a parent from the given index.</summary>
            /// <param name="index">The index of the parent to remove.</param>
            public void RemoveAt(int index);
        }

        /// <summary>This is a variable parent container.</summary>
        /// <typeparam name="T">The type of the parent that is being set.</typeparam>
        private class VarParent<T> : IVarParent
                where T : class, IParent {
            private readonly List<T> source;

            /// <summary>Creates a new variable parent container using the given list.</summary>
            /// <param name="source">The list of parents to read from and modify.</param>
            public VarParent(List<T> source) => this.source = source;

            /// <summary>Gets the type of the parents.</summary>
            public S.Type Type => typeof(T);

            /// <summary>This gets the number of variable parents.</summary>
            public int Count => this.source.Count;

            /// <summary>This gets or sets the parent at the given index.</summary>
            /// <param name="index">The index for the parent to get or set.</param>
            /// <returns>The parent at the given index.</returns>
            public IParent this[int index] {
                get => this.source[index];
                set => this.source[index] = value as T;
            }

            /// <summary>This adds a new parent to the end of the variable parents.</summary>
            /// <param name="item">This is the parent to add.</param>
            public void Add(IParent item) => this.source.Add(item as T);

            /// <summary>This inserts a parent into the list at the given index.</summary>
            /// <param name="index">The index to insert the parent at.</param>
            /// <param name="parent">The parent to insert into the list.</param>
            public void Insert(int index, IParent parent) => this.source.Insert(index, parent as T);

            /// <summary>This removes a parent from the given index.</summary>
            /// <param name="index">The index of the parent to remove.</param>
            public void RemoveAt(int index) => this.source.RemoveAt(index);

            /// <summary>Gets the enumerator of the variable parents.</summary>
            /// <returns>The enumerator of the variable parents.</returns>
            public IEnumerator<IParent> GetEnumerator() => this.source.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        #endregion

        /// <summary>The parameters for each fixed parent in this collection.</summary>
        private readonly List<IFixedParent> fixedParents;

        /// <summary>The list of variable parents.</summary>
        /// <remarks>This will be null if there are no variable parents.</remarks>
        private IVarParent varParents;

        /// <summary>Creates a new parent collection.</summary>
        /// <param name="child">The child node that this collection is for.</param>
        /// <param name="fixedCapacity">
        /// The initial capacity for the fixed parent part.
        /// This should be set when this parent collection has a fixed part.
        /// </param>
        internal ParentCollection(IChild child, int fixedCapacity = 0) {
            this.Child = child;
            this.fixedParents = new List<IFixedParent>(fixedCapacity);
            this.varParents = null;
        }

        /// <summary>This adds a new parent parameter type to this fixed parameter list.</summary>
        /// <remarks>
        /// The fixed parents will be in the order that this method is called.
        /// Fixed parents will always be before the variable parent no matter the order.
        /// </remarks>
        /// <typeparam name="T">The type of the parent for this parameter.</typeparam>
        /// <param name="getParent">The getter to get the parent value from the child.</param>
        /// <param name="setParent">
        /// The setter to set a parent value to the child.
        /// This should directly set the member without additional processing on the current value.
        /// The current and new parent will have already been processes as needed before being set.
        /// </param>
        /// <returns>This returns the fixed parent which this was called on so that calls can be chained.</returns>
        internal ParentCollection With<T>(S.Func<T> getParent, S.Action<T> setParent)
            where T : class, IParent {
            this.fixedParents.Add(new FixedParent<T>(getParent, setParent));
            return this;
        }

        /// <summary>This sets a new parent parameter type to the variable parameter.</summary>
        /// <typeparam name="T">The type of the parent for this parameter.</typeparam>
        /// <param name="source">
        /// The source for all the variable parents. This list will be read from and modified
        /// so should be the list inside of the variable parent node.
        /// </param>
        /// <returns>This returns the fixed parent which this was called on so that calls can be chained.</returns>
        internal ParentCollection With<T>(List<T> source)
            where T : class, IParent {
            if (this.IsVariable)
                throw new Message("May only have a single variable parent section in the parent collection.").
                    With("child", this.Child).
                    With("fixed count", this.FixedCount);
            this.varParents = new VarParent<T>(source);
            return this;
        }

        /// <summary>Indicates if this collection has a variable parent part.</summary>
        public bool IsVariable => this.varParents is not null;

        /// <summary>The child that this parent collection is from.</summary>
        public IChild Child { get; }

        /// <summary>The set of type that each parent could have in this collection.</summary>
        /// <remarks>
        /// For nodes with variable length parents, this can be very long
        /// so use this with zip or limit the enumerations.
        /// </remarks>
        public IEnumerable<S.Type> Types =>
            this.fixedParents.Select(p => p.Type).Concat(Enumerable.Repeat(this.varParents.Type, 1000));

        /// <summary>This gets the enumerator for all the parents currently set.</summary>
        public IEnumerable<IParent> Parents =>
            this.fixedParents.Select(p => p.Node).Concat(this.varParents ?? Enumerable.Empty<IParent>());

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x),
        /// then the Sum node will return the 'x' parent twice.
        /// </remarks>
        /// <returns>An enumerator for all the parents.</returns>
        public IEnumerator<IParent> GetEnumerator() => this.Parents.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>The number of parents currently in the fixed part of the collection.</summary>
        public int FixedCount => this.fixedParents.Count;

        /// <summary>The number of parents currently in the variable part of the collection.</summary>
        public int VarCount => this.varParents?.Count ?? 0;

        /// <summary>The number of parents currently in the collection.</summary>
        public int Count => this.FixedCount + this.VarCount;

        /// <summary>This gets the type of the parent at the given index.</summary>
        /// <param name="index">The index to get the parent's type of.</param>
        /// <returns>The type of the parent at the given index.s</returns>
        public S.Type TypeAt(int index) =>
            index < 0 && !this.IsVariable && index >= this.FixedCount ?
                throw new Message("Index out of bounds of node's parent types.").
                    With("child", this.Child).
                    With("index", index).
                    With("fixed count", this.FixedCount).
                    With("has var", this.IsVariable) :
                index < this.FixedCount ? this.fixedParents[index].Type :
                this.varParents.Type;

        /// <summary>This gets or sets the parent at the given location.</summary>
        /// <remarks>This will throw an exception if out-of-bounds or wrong type.</remarks>
        /// <param name="index">The index to get or set from.</param>
        /// <returns>The parent gotten from the given index.</returns>
        public IParent this[int index] {
            get => index < 0 || index >= this.Count ?
                    throw new Message("Index out of bounds of node's parents.").
                        With("child", this.Child).
                        With("index", index).
                        With("total", this.Count).
                        With("fixed count", this.FixedCount).
                        With("has var", this.IsVariable).
                        With("var count", this.VarCount) :
                    index < this.FixedCount ? this.fixedParents[index].Node :
                    this.varParents[index - this.FixedCount];
            set {
                IParent parent = this[index]; // This also will check bounds
                if (ReferenceEquals(parent, value)) return;

                S.Type type = this.TypeAt(index);
                if (value.GetType().IsAssignableTo(type))
                    throw new Message("Incorrect type of a parent being set to a node.").
                        With("child", this.Child).
                        With("index", index).
                        With("expected type", type).
                        With("fixed count", this.FixedCount).
                        With("has var", this.IsVariable).
                        With("var count", this.VarCount).
                        With("gotten type", value.GetType());

                bool removed = parent?.RemoveChildren(this.Child) ?? false;
                if (index < this.FixedCount)
                    this.fixedParents[index].Node = value;
                else this.varParents[index - this.FixedCount] = value;
                if (removed) value?.AddChildren(this.Child);
            }
        }

        /// <summary>This will check if a replace would work and would make a change.</summary>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parents would be replaced, false otherwise.</returns>
        private bool checkReplace(IParent oldParent, IParent newParent) {
            if (ReferenceEquals(oldParent, newParent)) return false;

            bool wouldChange = false;
            foreach ((int index, S.Type type, IParent parent) in
                this.Types.Zip(this.Parents).Select((pair, index) => (index, pair.First, pair.Second))) {
                if (!ReferenceEquals(parent, oldParent) || ReferenceEquals(parent, newParent)) continue;
                wouldChange = true;

                if (newParent is not null && !newParent.GetType().IsAssignableTo(type))
                    throw new Message("Unable to replace old parent with new parent.").
                        With("child", this.Child).
                        With("node", parent).
                        With("index", index).
                        With("fixed count", this.FixedCount).
                        With("has var", this.IsVariable).
                        With("var count", this.VarCount).
                        With("old parent", oldParent).
                        With("new parent", newParent).
                        With("target type", type);

                // If we've checked one of the variable nodes, then we don't have to check anymore.
                if (index >= this.FixedCount) break;
            }
            return wouldChange;
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
            if (!this.checkReplace(oldParent, newParent)) return false;

            bool changed = false, removed = false;
            for (int i = this.fixedParents.Count - 1; i >= 0; --i) {
                IFixedParent parent = this.fixedParents[i];
                IParent node = parent.Node;
                if (!ReferenceEquals(node, oldParent) || ReferenceEquals(node, newParent)) continue;
                removed = node?.RemoveChildren(this.Child) ?? false;
                parent.Node = newParent;
                changed = true;
            }

            for (int i = this.varParents.Count - 1; i >= 0; --i) {
                IParent node = this.varParents[i];
                if (!ReferenceEquals(node, oldParent) || ReferenceEquals(node, newParent)) continue;
                removed = node?.RemoveChildren(this.Child) ?? false;
                this.varParents[i] = newParent;
                changed = true;
            }

            if (removed) newParent?.AddChildren(this.Child);
            return changed;
        }

        /// <summary>This checks if the setting all the parents would work and cause a change.</summary>
        /// <param name="newParents">The new parents that would be set.</param>
        /// <returns>True if any parent was changed, false otherwise.</returns>
        private bool checkSetAll(List<IParent> newParents) {
            int count = newParents.Count;
            if (count < this.FixedCount || (!this.IsVariable && count > this.FixedCount))
                throw new Message("Incorrect number of parents in the list of parents to set to a node.").
                    With("child", this.Child).
                    With("expected count", count).
                    With("given count", this.fixedParents.Count);

            bool wouldChange = false;
            foreach ((int index, S.Type type, IParent newParent, IParent curParent) in
                this.Types.Zip(this.Parents, newParents).Select((pair, index) => (index, pair.First, pair.Second, pair.Third))) {
                if (newParent.GetType().IsAssignableTo(type))
                    throw new Message("Incorrect type of a parent in the list of parents to set to a node.").
                        With("child", this.Child).
                        With("index", index).
                        With("fixed count", this.FixedCount).
                        With("has var", this.IsVariable).
                        With("expected type", type).
                        With("gotten type", newParent.GetType());

                wouldChange = wouldChange || !ReferenceEquals(curParent, newParent);
            }

            // TODO: Check all new parents that don't overlap with the old parents

            return wouldChange;
        }

        /// <summary>This attempts to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAll(List<IParent> newParents) {
            if (!this.checkSetAll(newParents)) return false;
            bool changed = false;

            // Update fixed parents
            int count = newParents.Count;
            for (int i = 0; i < this.FixedCount; ++i) {
                IParent newParent = newParents[i];
                IFixedParent param = this.fixedParents[i];
                IParent node = param.Node;
                if (ReferenceEquals(node, newParent)) continue;
                bool removed = node?.RemoveChildren(this.Child) ?? false;
                param.Node = newParent;
                if (removed) newParent?.AddChildren(this.Child);
                changed = true;
            }

            // Update variable parents which overlap with new parents
            int minCount = S.Math.Min(count, this.VarCount);
            for (int i = this.FixedCount, j = 0; i < minCount; ++i, ++j) {
                IParent newParent = newParents[i];
                IParent oldParent = this.varParents[i];
                if (!ReferenceEquals(oldParent, newParent)) {
                    bool removed = oldParent?.RemoveChildren(this.Child) ?? false;
                    if (removed) newParent?.AddChildren(this.Child);
                    this.varParents[i] = newParent;
                    changed = true;
                }
            }

            // Remove any old variable parents which are beyond the new parents
            for (int i = this.VarCount - 1; i >= minCount; --i) {
                this.varParents[i]?.RemoveChildren(this.Child);
                this.varParents.RemoveAt(i);
            }

            // Add any new parents which are beyond the old variable parents
            for (int i = this.VarCount; i < minCount; ++i) {
                IParent newParent = newParents[i];
                newParent?.AddChildren(this.Child);
                this.varParents.Add(newParent);
            }
            return changed;
        }

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
        public bool Insert(int index, IEnumerable<IParent> newParents, IChild oldChild = null) {



            return false;
        }

        /// <summary>This remove one or more parent at the given location.</summary>
        /// <remarks>This will throw an exception for fixed parent collections.</remarks>>
        /// <param name="index">The index to start removing the parents from.</param>
        /// <param name="length">The number of parents to remove.</param>
        public void Remove(int index, int length = 1) {



        }
    }
}