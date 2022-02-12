using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections {
    internal class FixedParents: IParentCollection {

        private interface IParam {
            public S.Type Type { get; }
            public IParent Node { get; set; }
        }

        private class Param<T>: IParam
                where T : class, IParent {

            private readonly S.Func<T> getParent;

            private readonly S.Action<T> setParent;

            public Param(S.Func<T> getParent, S.Action<T> setParent) {
                this.getParent = getParent;
                this.setParent = setParent;
            }

            public S.Type Type => typeof(T);

            public IParent Node {
                get => this.getParent();
                set => this.setParent(value as T);
            }
        }

        private List<IParam> source;

        public FixedParents(IChild child) {
            this.Child = child;
            this.source = new List<IParam>();
        }

        internal FixedParents With<T>(S.Func<T> getParent, S.Action<T> setParent)
            where T : class, IParent {
            this.source.Add(new Param<T>(getParent, setParent));
            return this;
        }

        public readonly IChild Child;

        /// <summary>The set of type that each parent could have in this collection.</summary>
        public IEnumerable<S.Type> Types => this.source.Select((p) => p.Type);

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
        /// </remarks>
        public IEnumerable<IParent> Nodes => this.source.Select((p) => p.Node).NotNull();

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
        public bool SetAllParents(List<IParent> newParents) {
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
    }
}
