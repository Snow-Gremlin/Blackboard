using Blackboard.Core.Caps;
using Blackboard.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core {

    /// <summary>The set of extensions for the nodes of the blackboard.</summary>
    static internal class Extensions {

        /// <summary>Adds only unique values into the given list.</summary>
        /// <typeparam name="T">The type of values in the list.</typeparam>
        /// <param name="list">The list to add values into.</param>
        /// <param name="values">The values to add to the list.</param>
        static public void AddUnique<T>(this List<T> list, params T[] values) =>
            AddUnique(list, values as IEnumerable<T>);

        /// <summary>Adds only unique values into the given list.</summary>
        /// <typeparam name="T">The type of values in the list.</typeparam>
        /// <param name="list">The list to add values into.</param>
        /// <param name="values">The values to add to the list.</param>
        static public void AddUnique<T>(this List<T> list, IEnumerable<T> values) {
            foreach (T value in values) {
                if (!list.Contains(value)) list.Add(value);
            }
        }

        /// <summary>Adds only unique values into the given list and returns the added values.</summary>
        /// <typeparam name="T">The type of values in the list.</typeparam>
        /// <param name="list">The list to add values into.</param>
        /// <param name="values">The values to add to the list.</param>
        /// <returns>The list of values which were added.</returns>
        static public List<T> AddWhichUnique<T>(this List<T> list, params T[] values) =>
            AddWhichUnique(list, values as IEnumerable<T>);

        /// <summary>Adds only unique values into the given list and returns the added values.</summary>
        /// <typeparam name="T">The type of values in the list.</typeparam>
        /// <param name="list">The list to add values into.</param>
        /// <param name="values">The values to add to the list.</param>
        /// <returns>The list of values which were added.</returns>
        static public List<T> AddWhichUnique<T>(this List<T> list, IEnumerable<T> values) {
            List<T> result = new();
            foreach (T value in values) {
                if (!list.Contains(value)) {
                    list.Add(value);
                    result.Add(value);
                }
            }
            return result;
        }

        /// <summary>Filters any null values out of the given enumerable.</summary>
        /// <typeparam name="T">The type of the values to check.</typeparam>
        /// <param name="input">The input to get all the values from.</param>
        /// <returns>The input values without any null values.</returns>
        static public IEnumerable<T> NotNull<T>(this IEnumerable<T> input) where T : class {
            foreach (T value in input) {
                if (value is null) continue;
                yield return value;
            }
        }

        /// <summary>The values from the given input values.</summary>
        /// <typeparam name="T">The type of the values to get.</typeparam>
        /// <param name="nodes">The set of nodes to get all the values from.</param>
        /// <returns>The values from the given non-null nodes.</returns>
        static public IEnumerable<T> Values<T>(this IEnumerable<IValue<T>> nodes) {
            foreach (IValue<T> node in nodes) {
                yield return node is null ? default : node.Value;
            }
        }

        /// <summary>The triggers from the given input nodes.</summary>
        /// <param name="nodes">The set of nodes to get all the triggers from.</param>
        /// <returns>The triggers from the given non-null nodes.</returns>
        static public IEnumerable<bool> Triggers(this IEnumerable<ITrigger> nodes) {
            foreach (ITrigger node in nodes) {
                yield return node?.Triggered ?? false;
            }
        }

        /// <summary>Determines if any of values exists in both lists.</summary>
        /// <typeparam name="T">The types of values to find.</typeparam>
        /// <param name="a">The first input set of values to check within.</param>
        /// <param name="b">The second input set of values to check against.</param>
        /// <param name="comparer">The type of comparer to use, if null the default comparer will be used.</param>
        /// <returns>True if any value is contained in both, false otherwise.</returns>
        static public bool ContainsAny<T>(this IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T> comparer = null) {
            foreach (T value in a) {
                if (b.Contains(value, comparer)) return true;
            }
            return false;
        }

        /// <summary>Gets the maximum depth from the given nodes.</summary>
        /// <param name="nodes">The nodes to get the maximum depth from.</param>
        /// <returns>The maximum found depth.</returns>
        static public int MaxDepth(this IEnumerable<INode> nodes) {
            int depth = 0;
            foreach (INode node in nodes)
                depth = Math.Max(depth, node.Depth);
            return depth;
        }

        /// <summary>Gets and removes the first value from the given linked list.</summary>
        /// <typeparam name="T">The type of the values in the list.</typeparam>
        /// <param name="list">The list to take the first value from.</param>
        /// <returns>The first value from the linked list or the default value if the list is empty.</returns>
        static public T TakeFirst<T>(this LinkedList<T> list) {
            if (list.Count <= 0) return default;
            T value = list.First.Value;
            list.RemoveFirst();
            return value;
        }

        /// <summary>This sort inserts unique values into the given linked list.</summary>
        /// <param name="list">The list of values to sort insert into.</param>
        /// <param name="nodes">The set of nodes to insert.</param>
        static public void SortInsertUnique(this LinkedList<INode> list, params INode[] nodes) =>
            list.SortInsertUnique(nodes as IEnumerable<INode>);

        /// <summary>This sort inserts unique values into the given linked list.</summary>
        /// <param name="list">The list of values to sort insert into.</param>
        /// <param name="nodes">The set of nodes to insert.</param>
        static public void SortInsertUnique(this LinkedList<INode> list, IEnumerable<INode> nodes) {
            foreach (INode node in nodes) {
                bool addToEnd = true;
                for (LinkedListNode<INode> pend = list.First; pend is not null; pend = pend.Next) {
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
    }
}
