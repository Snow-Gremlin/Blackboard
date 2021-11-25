using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Extensions {

    /// <summary>The set of extensions for working with IEvaluatable interfaces.</summary>
    static class IEvaluatableExt {

        /// <summary>This sort inserts unique nodes into the given linked list.</summary>
        /// <typeparam name="T">The type of evaulatable node being worked with.</typeparam>
        /// <param name="list">The list of values to sort insert into.</param>
        /// <param name="nodes">The set of nodes to insert.</param>
        static public void SortInsertUnique<T>(this LinkedList<T> list, params T[] nodes)
            where T : IEvaluatable =>
            list.SortInsertUnique(nodes as IEnumerable<T>);

        /// <summary>This sort inserts unique evaluatable nodes into the given linked list.</summary>
        /// <typeparam name="T">The type of evaulatable node being worked with.</typeparam>
        /// <param name="list">The list of values to sort insert into.</param>
        /// <param name="nodes">The set of nodes to insert.</param>
        static public void SortInsertUnique<T>(this LinkedList<T> list, IEnumerable<T> nodes)
            where T : IEvaluatable {
            foreach (T node in nodes) {
                bool addToEnd = true;
                for (LinkedListNode<T> pend = list.First; pend is not null; pend = pend.Next) {
                    if (ReferenceEquals(node, pend.Value)) {
                        addToEnd = false;
                        break;
                    }
                    if (node.Depth < pend.Value.Depth) {
                        list.AddBefore(pend, node);
                        addToEnd = false;
                        break;
                    }
                }
                if (addToEnd) list.AddLast(node);
            }
        }

        /// <summary>Gets the maximum depth from the given nodes.</summary>
        /// <param name="nodes">The nodes to get the maximum depth from.</param>
        /// <returns>The maximum found depth.</returns>
        static public int MaxDepth(this IEnumerable<IEvaluatable> nodes) =>
            nodes.Select((node) => node.Depth).Aggregate(0, S.Math.Max);
    }
}
