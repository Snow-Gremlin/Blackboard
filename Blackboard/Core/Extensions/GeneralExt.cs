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
    }
}
