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

        /// <summary>Filters a sequence of values based on the inverse of the predicate.</summary>
        /// <typeparam name="T">The type of values being filtered.</typeparam>
        /// <param name="input">The values to filter.</param>
        /// <param name="predicate">The predicate to filter if not passing.</param>
        /// <returns>The filtered values.</returns>
        static public IEnumerable<T> WhereNot<T>(this IEnumerable<T> input, S.Func<T, bool> predicate) =>
            input.Where(value => !predicate(value));

        /// <summary>Runs all the values on a the given predicate.</summary>
        /// <typeparam name="T">The type of values being run.</typeparam>
        /// <param name="input">The values to run.</param>
        /// <param name="predicate">The predicate to run on each value.</param>
        static public void Foreach<T>(this IEnumerable<T> input, S.Action<T> predicate) {
            foreach (T value in input) predicate(value);
        }

        /// <summary>Filters any null values out of the given enumerable.</summary>
        /// <typeparam name="T">The type of the values to check.</typeparam>
        /// <param name="input">The input to get all the values from.</param>
        /// <returns>The input values without any null values.</returns>
        static public IEnumerable<T> NotNull<T>(this IEnumerable<T> input)
            where T : class =>
            from value in input where value is not null select value;

        /// <summary>Determines if any of values exists in both lists.</summary>
        /// <typeparam name="T">The types of values to find.</typeparam>
        /// <param name="a">The first input set of values to check within.</param>
        /// <param name="b">The second input set of values to check against.</param>
        /// <param name="comparer">The type of comparer to use, if null the default comparer will be used.</param>
        /// <returns>True if any value is contained in both, false otherwise.</returns>
        static public bool ContainsAny<T>(this IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T> comparer = null) =>
            a.Any((value) => b.Contains(value, comparer));

        /// <summary>This will enumerate the given list then for each additional call it will return the last value.</summary>
        /// <remarks>This will continue forever so you must provide another part of the enumerations to stop it, such as a zip.</remarks>
        /// <typeparam name="T">The type of the list to enumerate.</typeparam>
        /// <param name="values">The values to enumerate and then repeat the last of.</param>
        /// <returns>The enumeration of the values and repeated last. If no input values then default is returned.</returns>
        static public IEnumerable<T> RepeatLast<T>(this IEnumerable<T> values) {
            T prev = default;
            foreach (T value in values) {
                prev = value;
                yield return value;
            }
            while (true) yield return prev;
        }

        /// <summary>The strings for all the given values.</summary>
        /// <typeparam name="T">The types of values to stringify.</typeparam>
        /// <param name="values">The values to get the string for.</param>
        /// <param name="nullStr">The string to return if the value is null.</param>
        /// <returns>The strings for the given values.</returns>
        static public IEnumerable<string> Strings<T>(this IEnumerable<T> values, string nullStr = "null") =>
            from v in values select v?.ToString() ?? nullStr;

        /// <summary>This will indent all the lines in the given strings with the given indent.</summary>
        /// <param name="parts">The string to indent all the lines with.</param>
        /// <param name="indent">The indent to add to the front of each line.</param>
        /// <returns>The parts all indented.</returns>
        static public IEnumerable<string> Indent(this IEnumerable<string> parts, string indent) =>
            from p in parts select p.Replace("\n", "\n" + indent);

        /// <summary>This is short hand to make joining strings into one.</summary>
        /// <typeparam name="T">The types of values to stringify and join.</typeparam>
        /// <param name="parts">The strings to join together into one.</param>
        /// <param name="seperator">The separator to put between the parts.</param>
        /// <returns>The string from the joined parts.</returns>
        static public string Join<T>(this IEnumerable<T> parts, string seperator = null) =>
            string.Join(seperator, parts);

        /// <summary>This is short hand to make joining strings into one.</summary>
        /// <typeparam name="T">The types of values to stringify and join.</typeparam>
        /// <param name="parts">The strings to join together into one.</param>
        /// <param name="seperator">The separator to put between the parts.</param>
        /// <returns>The string from the joined parts.</returns>
        static public string Join<T>(this IEnumerable<T> parts, char seperator) =>
            string.Join(seperator, parts);

        #endregion
        #region Specifics...

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

        /// <summary>This will split and trim the string by the give separator. It will filter out empty strings.</summary>
        /// <param name="src">The source string to split and trim.</param>
        /// <param name="separator">The string to split the source on.</param>
        /// <returns>All the trimmed parts which were not empty.</returns>
        static public IEnumerable<string> SplitAndTrim(this string src, string separator) =>
            src.Split(separator).Select(name => name.Trim()).Where(name => !string.IsNullOrEmpty(name));

        /// <summary>This will split and trim the string by the give separator. It will filter out empty strings.</summary>
        /// <param name="src">The source string to split and trim.</param>
        /// <param name="separator">The character to split the source on.</param>
        /// <returns>All the trimmed parts which were not empty.</returns>
        static public IEnumerable<string> SplitAndTrim(this string src, char separator) =>
            src.Split(separator).Select(name => name.Trim()).Where(name => !string.IsNullOrEmpty(name));

        #endregion
        #region Data...

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
        static public bool CanReachAny(this IChild root, params INode[] targets) =>
            root.CanReachAny(targets as IEnumerable<INode>);

        /// <summary>This will check if from the given root node any of the given target nodes can be reachable.</summary>
        /// <param name="root">The root to start checking from.</param>
        /// <param name="targets">The target nodes to try to reach.</param>
        /// <returns>True if any of the targets can be reached, false otherwise.</returns>
        static public bool CanReachAny(this IChild root, IEnumerable<INode> targets) {
            if (root is null) return true;
            HashSet<IChild> touched = new();
            Queue<IChild> pending = new();
            pending.Enqueue(root);
            touched.Add(root);

            while (pending.Count > 0) {
                IChild node = pending.Dequeue();
                if (targets.Contains(node)) return true;

                foreach (IChild child in node.Parents.NotNull().OfType<IChild>().WhereNot(touched.Contains)) {
                    pending.Enqueue(child);
                    touched.Add(child);
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
        static public bool IsConstant<T>(this IEnumerable<T> nodes) where T: INode =>
            nodes.All(node => node is IConstant);

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
