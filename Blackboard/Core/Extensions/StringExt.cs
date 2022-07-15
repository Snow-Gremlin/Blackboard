using System.Collections.Generic;
using System.Linq;
using PetiteParser.Diff;

namespace Blackboard.Core.Extensions {

    /// <summary>The set of extensions for working with strings.</summary>
    static public class StringExt {

        /// <summary>The strings for all the given values.</summary>
        /// <typeparam name="T">The types of values to convert into strings.</typeparam>
        /// <param name="values">The values to get the string for.</param>
        /// <param name="nullStr">The string to return if the value is null.</param>
        /// <returns>The strings for the given values.</returns>
        static public IEnumerable<string> Strings<T>(this IEnumerable<T> values, string nullStr = "null") =>
            from v in values select v?.ToString() ?? nullStr;


        /// <summary>This will indent all the lines in the given string with the given indent.</summary>
        /// <param name="part">The string to indent all the lines with.</param>
        /// <param name="indent">The indent to add to the front of each line.</param>
        /// <returns>The indented lines of the string.</returns>
        static public string Indent(this string part, string indent) =>
            indent + part.Replace("\n", "\n" + indent);

        /// <summary>This will indent all the lines in the given strings with the given indent.</summary>
        /// <param name="parts">The strings to indent all the lines with.</param>
        /// <param name="indent">The indent to add to the front of each line.</param>
        /// <returns>The parts all indented.</returns>
        static public IEnumerable<string> Indent(this IEnumerable<string> parts, string indent) =>
            from p in parts select p.Indent(indent);

        /// <summary>This is short hand to make joining strings into one.</summary>
        /// <typeparam name="T">The types of values to convert into strings and join.</typeparam>
        /// <param name="parts">The strings to join together into one.</param>
        /// <param name="seperator">The separator to put between the parts.</param>
        /// <returns>The string from the joined parts.</returns>
        static public string Join<T>(this IEnumerable<T> parts, string seperator = null) =>
            string.Join(seperator, parts);

        /// <summary>This is short hand to make joining strings into one.</summary>
        /// <typeparam name="T">The types of values to convert into strings and join.</typeparam>
        /// <param name="parts">The strings to join together into one.</param>
        /// <param name="seperator">The separator to put between the parts.</param>
        /// <returns>The string from the joined parts.</returns>
        static public string Join<T>(this IEnumerable<T> parts, char seperator) =>
            string.Join(seperator, parts);

        /// <summary>This will split all of the strings by the given separator and return all of the strings.</summary>
        /// <remarks>This is useful for dealing with a collection of lines as strings which might be multi-lined.</remarks>
        /// <param name="src">The input strings to split by the separator.</param>
        /// <param name="separator">The separator to split with.</param>
        /// <returns>All the string separated by the parts.</returns>
        static public IEnumerable<string> Split(this IEnumerable<string> src, string separator) =>
            src.SelectMany(part => part.Split(separator));

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

        /// <summary>The singleton to reuse any allocated diff memory.</summary>
        static private readonly System.Lazy<Diff> diffSingleton = new();

        /// <summary>This will create a diff between the two given lists of values.</summary>
        /// <param name="first">The first list of values (added).</param>
        /// <param name="second">The second list of values (removed).</param>
        /// <returns>The lines of the diff as strings.</returns>
        static public IEnumerable<string> Diff<T>(this IReadOnlyList<T> first, IReadOnlyList<T> second) =>
            diffSingleton.Value.PlusMinus(first, second);

        /// <summary>This will create a diff between the two given lists of values.</summary>
        /// <param name="first">The first list of values (added).</param>
        /// <param name="second">The second list of values (removed).</param>
        /// <returns>The lines of the diff as strings.</returns>
        static public IEnumerable<string> Diff<T>(this IEnumerable<T> first, IEnumerable<T> second) =>
            diffSingleton.Value.PlusMinus(first.ToArray(), second.ToArray());
    }
}
