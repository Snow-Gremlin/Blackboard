using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Collections {

    /// <summary>This is a collection of parent nodes for an IChild.</summary>
    /// <remarks>
    /// This should not contain null parents, but it might contain repeat parents.
    /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
    /// Since null parents are removed, this list may not be the same as the Count even if fixed.
    /// </remarks>
    public class ParentCollection : IEnumerable<IParent> {

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

            /// <summary>
            /// Gets or sets the parent.
            /// This parent must be able to case to the container type.
            /// </summary>
            public IParent Node {
                get => this.getParent();
                set => this.setParent(value as T);
            }
        }

        /// <summary>The definition for all variable parent container.</summary>
        private interface IVarParent : IEnumerable<IParent> {

            /// <summary>Gets the type of the container.</summary>
            public S.Type Type { get; }

            public int Count { get; }

            public IParent this[int index] { get; set; }

            public void Add(IParent item);

            public void Insert(int index, IParent item);

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

            public int Count => this.source.Count;

            public IParent this[int index] {
                get => this.source[index];
                set => this.source[index] = value as T;
            }

            public void Add(IParent item) => this.source.Add(item as T);

            public void Insert(int index, IParent item) => this.source.Insert(index, item as T);

            public void RemoveAt(int index) => this.source.RemoveAt(index);

            public IEnumerator<IParent> GetEnumerator() => this.source.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        /// <summary>The parameters for each fixed parent in this collection.</summary>
        private readonly List<IFixedParent> fixedParents;

        private IVarParent varParents;

        internal ParentCollection(IChild child, int fixedCapacity = 0) {
            this.Child = child;
            this.fixedParents = new List<IFixedParent>(fixedCapacity);
            this.varParents = null;
        }

        /// <summary>This adds a new parent parameter type to this fixed parameter list.</summary>
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
            // TODO: Check if var is set.
            this.fixedParents.Add(new FixedParent<T>(getParent, setParent));
            return this;
        }

        internal ParentCollection With<T>(List<T> source)
            where T : class, IParent {
            // TODO: Check if var is set.
            this.varParents = new VarParent<T>(source);
            return this;
        }

        public bool IsVariable => this.varParents is not null;

        /// <summary>The child that this parent collection is from.</summary>
        public IChild Child { get; }

        /// <summary>The set of type that each parent could have in this collection.</summary>
        /// <remarks>For nodes with variable length parents, this can be very long so use this with zip or limit the enumerations.</remarks>
        public IEnumerable<S.Type> Types {
            get {
                foreach (IFixedParent fixedParent in this.fixedParents)
                    yield return fixedParent.Type;

                if (this.IsVariable) {
                    for (int i = 0; i < 10000; ++i)
                        yield return this.varParents.Type;
                }
            }
        }

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x),
        /// then the Sum node will return the 'x' parent twice.
        /// </remarks>
        /// <returns>An enumerator for all the parents.</returns>
        public IEnumerator<IParent> GetEnumerator() {
            foreach (IFixedParent fixedParent in this.fixedParents)
                yield return fixedParent.Node;

            if (this.IsVariable) {
                foreach (IParent parent in this.varParents)
                    yield return parent;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>The number of parents currently in the fixed part of the collection.</summary>
        public int FixedCount => this.fixedParents.Count;

        /// <summary>The number of parents currently in the variable part of the collection.</summary>
        public int VarCount => this.varParents?.Count ?? 0;

        /// <summary>The number of parents currently in the collection.</summary>
        public int Count => this.FixedCount + this.VarCount;

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
                        With("gotten type", value.GetType());

                bool removed = parent?.RemoveChildren(this.Child) ?? false;
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


            return false;
        }

        /// <summary>This attempts to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAll(List<IParent> newParents) {

            return false;
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