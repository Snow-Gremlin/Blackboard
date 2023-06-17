using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Extensions;

/// <summary>The set of extensions for working with lists.</summary>
static public class ListExt {

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
    static public T? TakeFirst<T>(this LinkedList<T> list) {
        LinkedListNode<T>? first = list.First;
        if (first is null) return default;
        T value = first.Value;
        list.RemoveFirst();
        return value;
    }
}
