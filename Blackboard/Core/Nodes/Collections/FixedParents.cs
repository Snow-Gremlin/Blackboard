using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections {

    /// <summary>
    /// This is a parent collection for a child which has a
    /// fixed number of parents (unary, binary, and ternary).
    /// </summary>
    internal class FixedParents: IParentCollection {

        /// <summary>The definition for a single parent parameter.</summary>
        private interface IParam {

            /// <summary>Gets the type of the parameter.</summary>
            public S.Type Type { get; }
            public IParent Node { get; set; }
        }

        /// <summary>This is a single parent parameter.</summary>
        /// <typeparam name="T">The type of the parent that is being set.</typeparam>
        private class Param<T>: IParam
                where T : class, IParent {

            private readonly S.Func<T> getParent;
            private readonly S.Action<T> setParent;

            /// <summary>Creates a new parent parameter using the given Getter or Setter.</summary>
            /// <param name="getParent">The getter to get the parent value from the child.</param>
            /// <param name="setParent">The setter to set a parent value to the child.</param>
            public Param(S.Func<T> getParent, S.Action<T> setParent) {
                this.getParent = getParent;
                this.setParent = setParent;
            }

            /// <summary>Gets the type of the parameter.</summary>
            public S.Type Type => typeof(T);

            /// <summary>
            /// Gets or sets the parent.
            /// This parent must be able to case to the current type.
            /// </summary>
            public IParent Node {
                get => this.getParent();
                set => this.setParent(value as T);
            }
        }

        /// <summary>The parameters for each parent in this fixed collection.</summary>
        private List<IParam> source;

        /// <summary>Constructs a new fixed parent collection.</summary>
        /// <remarks>This is created with no parents. Use the `With` method to add a parent parameter.</remarks>
        /// <param name="child">The child this parent collection is for.</param>
        public FixedParents(IChild child) {
            this.Child = child;
            this.source = new List<IParam>();
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
        internal FixedParents With<T>(S.Func<T> getParent, S.Action<T> setParent)
            where T : class, IParent {
            this.source.Add(new Param<T>(getParent, setParent));
            return this;
        }

        /// <summary>The child that this parent collection is from.</summary>
        public IChild Child { get; }

        /// <summary>The set of type that each parent could have in this collection.</summary>
        public IEnumerable<S.Type> Types => this.source.Select((p) => p.Type);

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
        /// </remarks>
        public IEnumerable<IParent> Nodes => this.source.Select((p) => p.Node).NotNull();

        /// <summary>Indicates that this collection is fixed.</summary>
        public bool Fixed => true;

        /// <summary>This is the number of parents in the collection.</summary>
        /// <remarks>Nodes may return a smaller number if any parent is null.</remarks>
        public int Count => this.source.Count;

        /// <summary>This gets or sets the parent at the given location.</summary>
        /// <remarks>This will throw an exception if out-of-bounds or wrong type.</remarks>
        /// <param name="index">The index to get or set from.</param>
        /// <returns>The parent gotten from the given index.</returns>
        public IParent this[int index] {
            get => this.source[index].Node;
            set {
                IParam param = this.source[index];
                if (ReferenceEquals(this.source[index].Node, value)) return;

                if (value.GetType().IsAssignableTo(param.Type))
                    throw new Exception("Incorrect type of a parent being set to a node.").
                        With("child", this.Child).
                        With("index", index).
                        With("expected type", param.Type).
                        With("gotten type", value.GetType());

                bool removed = param.Node?.RemoveChildren(this.Child) ?? false;
                param.Node = value;
                if (removed) value?.AddChildren(this.Child);
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
            if (!ReferenceEquals(oldParent, newParent)) return false;

            for (int i = this.source.Count - 1; i >= 0; --i) {
                IParam param = this.source[i];
                IParent node = param.Node;
                if (!ReferenceEquals(node, oldParent)) continue;
                if (newParent is not null && !newParent.GetType().IsAssignableTo(param.Type))
                    throw new Exception("Unable to replace old parent with new parent.").
                        With("child", this.Child).
                        With("node", node).
                        With("index", i).
                        With("old parent", oldParent).
                        With("new parent", newParent).
                        With("target type", param.Type);
            }

            bool changed = false;
            bool removed = false;
            for (int i = this.source.Count - 1; i >= 0; --i) {
                IParam param = this.source[i];
                if (!ReferenceEquals(param.Node, oldParent)) continue;
                IParent node = param.Node;
                if (ReferenceEquals(node, newParent)) continue;
                removed = node?.RemoveChildren(this.Child) ?? false;
                param.Node = newParent;
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
            int count = newParents.Count;
            if (this.source.Count != count)
                throw new Exception("Incorrect number of parents in the list of parents to set to a node.").
                    With("child", this.Child).
                    With("expected count", count).
                    With("given count", this.source.Count);

            for (int i = 0; i < count; ++i) {
                IParent newParent = newParents[i];
                IParam param = this.source[i];
                if (newParent.GetType().IsAssignableTo(param.Type))
                    throw new Exception("Incorrect type of a parent in the list of parents to set to a node.").
                        With("child", this.Child).
                        With("index", i).
                        With("expected type", param.Type).
                        With("gotten type", newParent.GetType());
            }

            bool changed = false;
            for (int i = 0; i < count; ++i) {
                IParent newParent = newParents[i];
                IParam param = this.source[i];
                IParent node = param.Node;
                if (ReferenceEquals(node, newParent)) return false;
                bool removed = node?.RemoveChildren(this.Child) ?? false;
                param.Node = newParent;
                if (removed) newParent?.AddChildren(this.Child);
                changed = true;
            }
            return changed;
        }

        /// <summary>This throws an exception because the collection is fixed.</summary>
        /// <param name="index">The index to insert the parents at.</param>
        /// <param name="newParents">The new parents to insert.</param>
        /// <param name="oldChild">The old child these parents are being moved from.</param>
        /// <returns>Nothing is returned because this will throw an exception.</returns>
        public bool Insert(int index, IEnumerable<IParent> newParents, IChild oldChild = null) =>
            throw new Exception("May not use Insert on a fixed parent collection.").
                With("index", index).
                With("new parents", newParents).
                With("new child", this.Child).
                With("old child", oldChild);

        /// <summary>This throws an exception because the collection is fixed.</summary>>
        /// <param name="index">The index to start removing the parents from.</param>
        /// <param name="length">The number of parents to remove.</param>
        public void Remove(int index, int length = 1) =>
            throw new Exception("May not use Remove on a fixed parent collection.").
                With("index", index).
                With("length", length).
                With("child", this.Child);
    }
}
