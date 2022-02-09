using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
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

        public IEnumerable<S.Type> Types => new S.Type[1] { typeof(T) }.RepeatLast();

        public IEnumerable<IParent> Nodes => this.source.NotNull();

        public bool ReplaceParent(IParent oldParent, IParent newParent) => throw new S.NotImplementedException();

        public bool SetAllParents(List<IParent> newParents) => throw new S.NotImplementedException();

        /*
        /// <summary>This replaces all instances of given parent from this node collection.</summary>
        /// <typeparam name="T">The type of the parents to replace.</typeparam>
        /// <param name="sources">The node source list to replace the parents inside of.</param>
        /// <param name="child">The child these sources are for.</param>
        /// <param name="oldParent">The old parent to replace.</param>
        /// <param name="newParent">The new parent to replace the old parent with.</param>
        /// <returns>True if any of the parents are replaced, false if none were removed.</returns>
        static internal bool ReplaceParents<T>(this List<T> sources, IChild child, IParent oldParent, IParent newParent)
            where T : class, IParent {
            bool replaced = false;
            bool typeChecked = false;
            for (int i = sources.Count - 1; i >= 0; i--) {
                T node = sources[i];
                if (!ReferenceEquals(node, oldParent)) continue;

                // Now that at least one parent will be replaced, check that the new parent can be used.
                if (!typeChecked && newParent is not null and not T)
                    throw new Exception("Unable to replace old parent with new parent in a list.").
                        With("child", child).
                        With("index", i).
                        With("node", node).
                        With("old Parent", oldParent).
                        With("new Parent", newParent);
                typeChecked = true;

                // Replace parent in list of sources.
                node?.RemoveChildren(child);
                sources[i] = newParent as T;
                replaced = true;
            }
            return replaced;
        }

        /// <summary>This will attempt to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct types.</remarks>
        /// <typeparam name="T">The type of the parents to set.</typeparam>
        /// <param name="sources">The node source list to replace the parents inside of.</param>
        /// <param name="child">The child these sources are for.</param>
        /// <param name="newParents">The parents to set.</param>
        static internal bool SetAllParents<T>(this List<T> sources, IChild child, List<IParent> newParents)
            where T : IParent {
            IChild.CheckParentsBeingSet(newParents, true, typeof(T));
            int oldCount = sources.Count;
            int newCount = newParents.Count;
            int minCount = S.Math.Min(oldCount, newCount);

            bool changed = oldCount != newCount;
            for (int i = 0; i < minCount; ++i) {
                if (!ReferenceEquals(sources[i], newParents[i])) {
                    bool removed = sources[i]?.RemoveChildren(child) ?? false;
                    if (removed) newParents[i]?.AddChildren(child);
                    changed = true;
                }
            }

            for (int i = minCount; i < oldCount; ++i)
                sources[i]?.RemoveChildren(child);
            sources.Clear();
            sources.AddRange(newParents.OfType<T>());
            return changed;
        }
        */
    }
}
