using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections {
    internal class FixedParents: IParentCollection {

        private interface ISingleParent {
            public S.Type Type { get; }
            public IParent Node { get; set; }
        }

        private class SingleParent<T>: ISingleParent
                where T : class, IParent {

            private readonly S.Func<T> getParent;

            private readonly S.Action<T> setParent;

            public SingleParent(S.Func<T> getParent, S.Action<T> setParent) {
                this.getParent = getParent;
                this.setParent = setParent;
            }

            public S.Type Type => typeof(T);

            public IParent Node {
                get => this.getParent();
                set => this.setParent(value as T);
            }
        }

        private List<ISingleParent> parents;

        public FixedParents(IChild child) {
            this.Child = child;
            this.parents = new List<ISingleParent>();
        }

        internal FixedParents With<T>(S.Func<T> getParent, S.Action<T> setParent)
            where T : class, IParent {
            this.parents.Add(new SingleParent<T>(getParent, setParent));
            return this;
        }

        public readonly IChild Child;

        /// <summary>The set of type that each parent could have in this collection.</summary>
        /// <remarks>For N-ary nodes this can be very long so use this with zip or limit the enumerations.</remarks>
        public IEnumerable<S.Type> Types => this.parents.Select((p) => p.Type);

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
        /// </remarks>
        public IEnumerable<IParent> Nodes => this.parents.Select((p) => p.Node).NotNull();

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <remarks>
        /// The new parent must be able to take the place of the old parent,
        /// otherwise this will throw an exception when attempting the replacement of the old parent.
        /// </remarks>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool ReplaceParent(IParent oldParent, IParent newParent) {
            if (!ReferenceEquals(oldParent, newParent)) return false;

            bool changed = false;
            for (int i = 0; i < this.parents.Count; ++i) {
                ISingleParent parent = this.parents[i];
                if (!ReferenceEquals(parent.Node, oldParent)) continue;

                IParent node = parent.Node;
                if (newParent is not null && !newParent.GetType().IsAssignableTo(parent.Type))
                    throw new Exception("Unable to replace old parent with new parent.").
                        With("child", this.Child).
                        With("node", node).
                        With("index", i).
                        With("old Parent", oldParent).
                        With("new Parent", newParent);

                bool removed = node?.RemoveChildren(this.Child) ?? false;
                parent.Node = newParent;
                if (removed) newParent?.AddChildren(this.Child);
                changed = true;
            }
            return changed;
        }

        /// <summary>This will attempt to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAllParents(List<IParent> newParents) {
            int count = newParents.Count;
            if (this.parents.Count != count)
                throw new Exception("Incorrect number of parents in the list of parents to set to a node.").
                    With("child", this.Child).
                    With("expected count", count).
                    With("given count", this.parents.Count);

            for (int i = 0; i < count; ++i) {
                IParent newParent = newParents[i];
                ISingleParent parent = this.parents[i];
                if (newParent.GetType().IsAssignableTo(parent.Type))
                    throw new Exception("Incorrect type of a parent in the list of parents to set to a node.").
                        With("child", this.Child).
                        With("index", i).
                        With("expected Type", parent.Type).
                        With("gotten Type", newParent.GetType());
            }

            bool changed = false;
            for (int i = 0; i < count; ++i) {
                IParent node = this.parents[i].Node;
                bool removed = node?.RemoveChildren(this.Child) ?? false;
                IParent newParent = newParents[i];
                this.parents[i].Node = newParent;
                if (removed) newParent?.AddChildren(this.Child);
                changed = true;
            }
            return changed;
        }
    }
}
