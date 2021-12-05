using System.Collections.Generic;
using System.Linq;

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

        /// <summary>This will indent all the lines in the given strings with the given indent.</summary>
        /// <param name="parts">The string to indent all the lines with.</param>
        /// <param name="indent">The indent to add to the front of each line.</param>
        /// <returns>The parts all indented.</returns>
        static public IEnumerable<string> Indent(this IEnumerable<string> parts, string indent) =>
            from p in parts select p.Replace("\n", "\n" + indent);

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
    }
}
