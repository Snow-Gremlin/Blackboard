using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core {

    /// <summary>The set of extensions for the nodes of the blackboard.</summary>
    static public class Extensions {
        #region General...

        /// <summary>Filters any null values out of the given enumerable.</summary>
        /// <typeparam name="T">The type of the values to check.</typeparam>
        /// <param name="input">The input to get all the values from.</param>
        /// <returns>The input values without any null values.</returns>
        static public IEnumerable<T> NotNull<T>(this IEnumerable<T> input)
            where T : class =>
            from value in input where value is not null select value;

        /// <summary>The strings for all the given values.</summary>
        /// <typeparam name="T">The types of values to stringify.</typeparam>
        /// <param name="values">The values to get the string for.</param>
        /// <param name="nullStr">The string to return if the value is null.</param>
        /// <returns>The strings for the given values.</returns>
        static public IEnumerable<string> Strings<T>(this IEnumerable<T> values, string nullStr = "null") =>
            from v in values select v?.ToString() ?? nullStr;

        /// <summary>Determines if any of values exists in both lists.</summary>
        /// <typeparam name="T">The types of values to find.</typeparam>
        /// <param name="a">The first input set of values to check within.</param>
        /// <param name="b">The second input set of values to check against.</param>
        /// <param name="comparer">The type of comparer to use, if null the default comparer will be used.</param>
        /// <returns>True if any value is contained in both, false otherwise.</returns>
        static public bool ContainsAny<T>(this IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T> comparer = null) =>
            a.Any((value) => b.Contains(value, comparer));

        /// <summary>This will indent all the lines in the given strings with the given indent.</summary>
        /// <param name="parts">The string to indent all the lines with.</param>
        /// <param name="indent">The indent to add to the front of each line.</param>
        /// <returns>The parts all indented.</returns>
        static public IEnumerable<string> Indent(this IEnumerable<string> parts, string indent) =>
            from p in parts select p.Replace("\n", "\n" + indent);

        /// <summary>This is short hand to make joining strings into one.</summary>
        /// <param name="parts">The strings to join together into one.</param>
        /// <param name="seperator">The separator to put between the parts.</param>
        /// <returns>The string from the joined parts.</returns>
        static public string Join(this IEnumerable<string> parts, string seperator = "") =>
            string.Join(seperator, parts);

        #endregion
        #region List...

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
        static public void AddUnique<T>(this List<T> list, IEnumerable<T> values) =>
            list.AddRange(from value in values where !list.Contains(value) select value);

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

        /// <summary>This sort inserts unique evaluatable nodes into the given linked list.</summary>
        /// <typeparam name="T">The type of evaulatable node being worked with.</typeparam>
        /// <param name="list">The list of values to sort insert into.</param>
        /// <param name="nodes">The set of nodes to insert.</param>
        static public void SortInsertUniqueEvaluatable<T>(this LinkedList<T> list, params T[] nodes)
            where T : IEvaluatable =>
            list.SortInsertUniqueEvaluatable(nodes as IEnumerable<T>);

        /// <summary>This sort inserts unique evaluatable nodes into the given linked list.</summary>
        /// <typeparam name="T">The type of evaulatable node being worked with.</typeparam>
        /// <param name="list">The list of values to sort insert into.</param>
        /// <param name="nodes">The set of nodes to insert.</param>
        static public void SortInsertUniqueEvaluatable<T>(this LinkedList<T> list, IEnumerable<T> nodes)
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

        #endregion
        #region Data...

        /// <summary>Performs an implicit cast from the given data value into the given type.</summary>
        /// <remarks>The types should be matched prior to being used in this method.</remarks>
        /// <typeparam name="T">The type to cast the value into.</typeparam>
        /// <param name="value">The value to cast into that type.</param>
        /// <returns>The new data value in the given type or default if unable to cast implicitly.</returns>
        static public T ImplicitCastTo<T>(this IData value) where T: IData =>
            value is T      v2 ? v2 :
            value is Bool   v3 ? v3 is IImplicit<Bool,   T> c3 ? c3.CastFrom(v3) : default :
            value is Double v4 ? v4 is IImplicit<Double, T> c4 ? c4.CastFrom(v4) : default :
            value is Int    v5 ? v5 is IImplicit<Int,    T> c5 ? c5.CastFrom(v5) : default :
            value is String v6 ? v6 is IImplicit<String, T> c6 ? c6.CastFrom(v6) : default :
            throw new Exception("Unexpected value type in implicit cast").
                With("Value Type", value.GetType());

        /// <summary>The values from the given input values.</summary>
        /// <typeparam name="T">The type of the values to get.</typeparam>
        /// <param name="nodes">The set of nodes to get all the values from.</param>
        /// <returns>The values from the given non-null nodes.</returns>
        static public IEnumerable<T> Values<T>(this IEnumerable<IValue<T>> nodes)
            where T : IData =>
            from node in nodes select node is not null ? node.Value : default;

        #endregion
        #region Nodes...

        /// <summary>This will check if from the given root node any of the given target nodes can be reachable.</summary>
        /// <param name="root">The root to start checking from.</param>
        /// <param name="targets">The target nodes to try to reach.</param>
        /// <returns>True if any of the targets can be reached, false otherwise.</returns>
        static public bool CanReachAny(this INode root, params INode[] targets) =>
            root.CanReachAny(targets as IEnumerable<INode>);

        /// <summary>This will check if from the given root node any of the given target nodes can be reachable.</summary>
        /// <param name="root">The root to start checking from.</param>
        /// <param name="targets">The target nodes to try to reach.</param>
        /// <returns>True if any of the targets can be reached, false otherwise.</returns>
        static public bool CanReachAny(this INode root, IEnumerable<INode> targets) {
            List<INode> touched = new();
            Queue<INode> pending = new();
            pending.Enqueue(root);
            while (pending.Count > 0) {
                INode node = pending.Dequeue();
                if (node is null) continue;
                touched.Add(node);
                if (targets.Contains(node)) return true;
                foreach (INode parent in node.Parents) {
                    if (!touched.Contains(parent)) pending.Enqueue(parent);
                }
            }
            return false;
        }

        /// <summary>The triggers from the given input nodes.</summary>
        /// <param name="nodes">The set of nodes to get all the triggers from.</param>
        /// <returns>The triggers from the given non-null nodes.</returns>
        static public IEnumerable<bool> Triggers(this IEnumerable<ITrigger> nodes) =>
            from node in nodes select node?.Provoked ?? false;

        /// <summary>This determines if all the given nodes are constant.</summary>
        /// <param name="nodes">The nodes to check if constant.</param>
        /// <returns>True if all nodes are constant, false otherwise.</returns>
        static public bool IsConstant(this IEnumerable<IConstantable> nodes) =>
            nodes.All((node) => node?.IsConstant ?? false);

        /// <summary>Gets the maximum depth from the given nodes.</summary>
        /// <param name="nodes">The nodes to get the maximum depth from.</param>
        /// <returns>The maximum found depth.</returns>
        static public int MaxDepth(this IEnumerable<IEvaluatable> nodes) =>
            nodes.Select((node) => node.Depth).Aggregate(0, S.Math.Max);

        #endregion
        #region Types...

        /// <summary>The type nodes for all the given nodes.</summary>
        /// <param name="nodes">The nodes to get the types for.</param>
        /// <returns>The types of the given nodes.</returns>
        static public IEnumerable<Type> Types(this IEnumerable<INode> nodes) =>
            from node in nodes select Type.TypeOf(node);

        /// <summary>The real types for all the given node types.</summary>
        /// <param name="types">The node types to get the real types for.</param>
        /// <returns>The real type for all the given types.</returns>
        static public IEnumerable<S.Type> RealTypes(this IEnumerable<Type> types) =>
            from t in types select t?.RealType ?? null;

        /// <summary>This returns the first type from the given list which the given source is assignable to.</summary>
        /// <param name="types">The types to check if the source assignable to.</param>
        /// <param name="source">The source type find the first assignable to type.</param>
        /// <returns>The first type the souce is assignable to or null if not found.</returns>
        static public Type FirstAssignable(this IEnumerable<Type> types, S.Type source) =>
            types.FirstOrDefault((t) => source.IsAssignableTo(t.RealType));

        #endregion
    }
}
