using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections {
    internal class VarParents<T>: IParentCollection
            where T : class, IParent {

        private readonly List<T> source;

        public VarParents(IChild child, List<T> source) {
            this.Child = child;
            this.source = source;
        }

        public readonly IChild Child;

        /// <summary>The set of type that each parent could have in this collection.</summary>
        /// <remarks>For Nary nodes this can be very long so use this with zip or limit the enumerations.</remarks>
        public IEnumerable<S.Type> Types => new S.Type[1] { typeof(T) }.RepeatLast();

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
        /// </remarks>
        public IEnumerable<IParent> Nodes => this.source.NotNull();

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <remarks>
        /// The new parent must be able to take the place of the old parent,
        /// otherwise this will throw an exception when attempting the replacement of the old parent.
        /// </remarks>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool ReplaceParent(IParent oldParent, IParent newParent) {
            bool changed = false;
            bool typeChecked = false;
            bool removed = false;
            for (int i = this.source.Count - 1; i >= 0; --i) {
                T node = this.source[i];
                if (!ReferenceEquals(node, oldParent)) continue;

                // Now that at least one parent will be replaced, check that the new parent can be used.
                if (!typeChecked && newParent is not null and not T)
                    throw new Exception("Unable to replace old parent with new parent in a list.").
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

        public bool SetAllParents(List<IParent> newParents) {
            //IChild.CheckParentsBeingSet(newParents, true, typeof(T));


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
    }
}
