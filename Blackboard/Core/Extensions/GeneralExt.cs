using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Extensions {

    /// <summary>The set of general use extensions.</summary>
    static public class GeneralExt {

        /// <summary>Filters a sequence of values based on the inverse of the predicate.</summary>
        /// <typeparam name="T">The type of values being filtered.</typeparam>
        /// <param name="input">The values to filter.</param>
        /// <param name="predicate">The predicate to filter if not passing.</param>
        /// <returns>The filtered values.</returns>
        static public IEnumerable<T> WhereNot<T>(this IEnumerable<T> input, S.Func<T, bool> predicate) =>
            input.Where(value => !predicate(value));

        /// <summary>Consumes all the values in the set by running the enumerable until the end.</summary>
        /// <typeparam name="T">The type of values being run.</typeparam>
        /// <param name="input">The values to run.</param>
        static public void Foreach<T>(this IEnumerable<T> input) {
            foreach (T _ in input) { }
        }

        /// <summary>Runs all the values on the given predicate.</summary>
        /// <typeparam name="T">The type of values being run.</typeparam>
        /// <param name="input">The values to run.</param>
        /// <param name="predicate">The predicate to run on each value.</param>
        static public void Foreach<T>(this IEnumerable<T> input, S.Action<T> predicate) {
            foreach (T value in input) predicate(value);
        }

        /// <summary>Runs all the values on the given predicate.</summary>
        /// <remarks>
        /// This is designed to make it easy to use methods with have a return value but the return isn't used.
        /// Instead of `data.Foreach(v => hashSet.Add(v))` it allows `data.Foreach(hashSet.Add)`.
        /// </remarks>
        /// <typeparam name="Tin">The type of values being run.</typeparam>
        /// <typeparam name="Tout">The type returned by the predicate, this is ignored.</typeparam>
        /// <param name="input">The values to run.</param>
        /// <param name="predicate">The predicate to run on each value.</param>
        static public void Foreach<Tin, Tout>(this IEnumerable<Tin> input, S.Func<Tin, Tout> predicate) {
            foreach (Tin value in input) predicate(value);
        }

        /// <summary>Runs all the key/value pairs on the given predicate.</summary>
        /// <typeparam name="T1">The type of keys being used.</typeparam>
        /// <typeparam name="T2">The type of values being used.</typeparam>
        /// <param name="input">The key/value pairs to run.</param>
        /// <param name="predicate">The predicate to run on each key/value pair.</param>
        static public void Foreach<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> input, S.Action<T1, T2> predicate) {
            foreach (KeyValuePair<T1, T2> pair in input) predicate(pair.Key, pair.Value);
        }

        /// <summary>Adds an index with the items from the given input.</summary>
        /// <typeparam name="T">The type of the input elements.</typeparam>
        /// <param name="input">The input values to add indices to.</param>
        /// <returns>The items from the given input paired with the index.</returns>
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> input) =>
            input.Select((item, index) => (item, index));

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
            a.Any(value => b.Contains(value, comparer));

        /// <summary>Determines if the given enumerator has the given count.</summary>
        /// <remarks>
        /// This is faster than `a.Count() == count` because it will shortcut
        /// if the enumerator is over the count to check against.
        /// </remarks>
        /// <typeparam name="T">The types of the value to count.</typeparam>
        /// <param name="a">The values to check the count with.</param>
        /// <param name="count">The count to check against.</param>
        /// <returns>True if there are the same number of given values as the given count</returns>
        static public bool IsCount<T>(this IEnumerable<T> a, int count) {
            if (a is ICollection<T> b) return b.Count == count;
            foreach (T value in a) {
                count--;
                if (count < 0) return false;
            }
            return count == 0;
        }

        /// <summary>This will enumerate the given list then for each additional call it will return the last value.</summary>
        /// <remarks>
        /// This will continue for the given maximum loops, so you must provide another part of the enumerations to stop it,
        /// such as a zip. The maximum number of loops is to prevent this from being an infinite loop. Set it to more than
        /// is reasonably going to be reached. For example, using this for a function type matching can limit to 1,000 because
        /// it is unreasonable to have a function with that many arguments being passed into it.
        /// </remarks>
        /// <typeparam name="T">The type of the list to enumerate.</typeparam>
        /// <param name="values">The values to enumerate and then repeat the last of.</param>
        /// <param name="maxLoops">The maximum number of times the last value is repeated.</param>
        /// <returns>The enumeration of the values and repeated last. If no input values then default is returned.</returns>
        static public IEnumerable<T> RepeatLast<T>(this IEnumerable<T> values, int maxLoops = 1000) {
            T last = default;
            foreach (T value in values) {
                last = value;
                yield return value;
            }
            for (int i = 0; i < maxLoops; ++i)
                yield return last;
        }

        /// <summary>This will expand several enumerable sets into one joined enumerable.</summary>
        /// <remarks>This is useful for using after a `Select` which returns an enumerable.</remarks>
        /// <typeparam name="T">The type of the list to enumerate.</typeparam>
        /// <param name="input">The set of enumerable sets to join together.</param>
        /// <returns>The single enumerable set with all the values from the input.</returns>
        static public IEnumerable<T> Expand<T>(this IEnumerable<IEnumerable<T>> input) =>
            input.SelectMany(i => i);

        /// <summary>Gets the given value clamped to the inclusive range of the given min and max.</summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>The value clamped between the given values.</returns>
        static public T Clamp<T>(this T value, T min, T max)
            where T : S.IComparable<T> =>
            value.CompareTo(min) < 0 ? min :
            value.CompareTo(max) > 0 ? max :
            value;

        /// <summary>Determines if the given value is in the inclusive range of the given min and max.</summary>
        /// <param name="value">The value to test.</param>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <returns>True if the value is in the given range, otherwise false.</returns>
        static public bool InRange<T>(this T value, T min, T max)
            where T : S.IComparable<T> =>
            value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }
}
