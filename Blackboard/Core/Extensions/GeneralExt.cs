﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using S = System;

namespace Blackboard.Core.Extensions;

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
    static public IEnumerable<T> NotNull<T>(this IEnumerable<T?> input)
        where T : class =>
        from value in input where value is not null select value;

    /// <summary>Determines if any of values exists in both lists.</summary>
    /// <typeparam name="T">The types of values to find.</typeparam>
    /// <param name="a">The first input set of values to check within.</param>
    /// <param name="b">The second input set of values to check against.</param>
    /// <param name="comparer">The type of comparer to use, if null the default comparer will be used.</param>
    /// <returns>True if any value is contained in both, false otherwise.</returns>
    static public bool ContainsAny<T>(this IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? comparer = null) =>
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
    static public IEnumerable<T?> RepeatLast<T>(this IEnumerable<T> values, int maxLoops = 1_000) {
        T? last = default;
        foreach (T value in values) {
            last = value;
            yield return value;
        }
        for (int i = 0; i < maxLoops; ++i)
            yield return last;
    }

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

    /// <summary>Gets a formatted type name for the given generic type.</summary>
    /// <param name="type">The type to get the formatted name for.</param>
    /// <returns>The formatted name of the given type.</returns>
    static public string FormattedTypeName(this S.Type type) {
        static void getTypeNamePart(StringBuilder buf, S.Type type) {
            if (type.IsGenericType) {
                string name = type.Name;
                int iBacktick = name.IndexOf('`');
                if (iBacktick > 0) name = name[..iBacktick];
                buf.Append(name);
                buf.Append('<');
                S.Type[] typeParameters = type.GetGenericArguments();
                getTypeNamePart(buf, typeParameters[0]);
                for (int i = 1; i < typeParameters.Length; ++i) {
                    buf.Append(", ");
                    getTypeNamePart(buf, typeParameters[i]);
                }
                buf.Append('>');
            } else buf.Append(type.Name);
        }

        StringBuilder buf = new();
        getTypeNamePart(buf, type);
        return buf.ToString();
    }

    /// <summary>
    /// Pads the given string with a padding string until the total length is at the given target total length.
    /// This will cut the padding to be at the total length but will not trim the given string such that
    /// if the given string starts out longer than the target length, the given string is returned unmodified.
    /// </summary>
    /// <param name="str">The string to pad.</param>
    /// <param name="totalLength">The target total length to pad up to.</param>
    /// <param name="padding">The padding string to repeat until the string is long enough.</param>
    /// <param name="left">True to indicate to pad the left side, False to pad the right.</param>
    /// <returns>The padded string.</returns>
    static public string PadString(this string str, int totalLength, string padding, bool left) {
        int count = str.Length;
        int padSize = padding.Length;
        if (count >= totalLength || padSize <= 0) return str;

        StringBuilder buf = new(totalLength);
        if (!left) buf.Append(str);

        for (int i = (totalLength - count) / padSize - 1; i >= 0; i--)
            buf.Append(padding);

        int diff = (totalLength - count) % padSize;
        if (diff > 0) buf.Append(padding[0..diff]);

        if (left) buf.Append(str);
        return buf.ToString();
    }
}
