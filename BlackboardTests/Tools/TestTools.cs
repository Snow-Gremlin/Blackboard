using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using S = System;

namespace BlackboardTests.Tools;

/// <summary>A collection of testing tools.</summary>
static public class TestTools {

    /// <summary>Check an exception from the given action.</summary>
    /// <param name="handle">The action to perform.</param>
    /// <param name="exp">The lines of the expected exception message.</param>
    static public void CheckException(S.Action handle, params string[] exp) {
        try {
            handle();
        } catch (S.Exception ex) {
            NoDiff(exp, ex.Message.Split("\n"));
            return;
        }
        Assert.Fail("Expected an exception to be thrown.\n{0}", exp.Join("\n"));
    }

    /// <summary>Asserts that all the given lines are equal, otherwise shows the diff.</summary>
    /// <param name="exp">The expected result to check against.</param>
    /// <param name="results">The result that is being checked.</param>
    /// <param name="message">The message to go along with the test.</param>
    static public void NoDiff(string exp, string results, string message = "") =>
        noDiff(exp.Split("\n").ToArray(), results.Split("\n").ToArray(), message);

    /// <summary>Asserts that all the given lists as strings are equal, otherwise shows the diff.</summary>
    /// <param name="exp">The expected result to check against.</param>
    /// <param name="results">The result that is being checked.</param>
    /// <param name="message">The message to go along with the test.</param>
    static public void NoDiff<T1, T2>(IEnumerable<string> exp, IEnumerable<string> results, string message = "") =>
        noDiff(exp.Split("\n").ToArray(), results.Split("\n").ToArray(), message);

    /// <summary>Asserts that all the given lines are equal, otherwise shows the diff.</summary>
    /// <param name="exp">The expected result to check against.</param>
    /// <param name="results">The result that is being checked.</param>
    /// <param name="message">The message to go along with the test.</param>
    static public void NoDiff<T>(IEnumerable<T> exp, IEnumerable<T> results, string message = "") =>
        noDiff(exp.ToArray(), results.ToArray(), message);

    /// <summary>Asserts that all the given values are equal, otherwise shows the diff.</summary>
    /// <param name="exp">The expected result to check against.</param>
    /// <param name="results">The result that is being checked.</param>
    /// <param name="message">The message to go along with the test.</param>
    static private void noDiff<T>(IReadOnlyList<T> exp, IReadOnlyList<T> results, string message = "") {
        if (!exp.SequenceEqual(results)) {
            StringBuilder buf = new();
            if (!string.IsNullOrEmpty(message)) {
                buf.AppendLine(message);
                buf.AppendLine();
            }

            buf.AppendLine("Diff:");
            results.Diff(exp).Indent("  ").Foreach(buf.AppendLine);
            buf.AppendLine();

            buf.AppendLine("Expected:");
            exp.Strings().Indent("  ").Foreach(buf.AppendLine);
            buf.AppendLine();

            buf.AppendLine("Results:");
            results.Strings().Indent("  ").Foreach(buf.AppendLine);
            buf.AppendLine();

            Assert.Fail(buf.ToString());
        }
    }

    /// <summary>Asserts that all the given values are equal, otherwise shows the diff.</summary>
    /// <param name="results">The result that is being checked.</param>
    /// <param name="exp">The expected result to check against.</param>
    static public void ValuesAreEqual<T>(IEnumerable<T> results, params T[] exp) =>
        noDiff(exp, results.ToArray());

    /// <summary>Check if the entries in the given sets of information match each other (repeats are ignored).</summary>
    /// <typeparam name="T">The type of the value to compare.</typeparam>
    /// <param name="exp">The set of expected values.</param>
    /// <param name="results">The results to check against the given values.</param>
    /// <param name="message">The message to go along with the test.</param>
    static public void SetEntriesMatch<T>(IEnumerable<T> exp, IEnumerable<T> results, string message = "") {
        HashSet<T> resultSet = results.ToHashSet();
        HashSet<T> expSet    = exp.ToHashSet();
        List<T> extra   = resultSet.WhereNot(expSet.Contains).ToList();
        List<T> missing = expSet.WhereNot(resultSet.Contains).ToList();
        if (extra.Count > 0 || missing.Count > 0) {
            StringBuilder buf = new();
            if (!string.IsNullOrEmpty(message))
                buf.AppendLine(message);

            if (extra.Count > 0) {
                extra.Sort();
                buf.AppendLine("Extra (" + extra.Count + "):");
                extra.Strings().Indent("  ").Foreach(buf.AppendLine);
            }

            if (missing.Count > 0) {
                missing.Sort();
                buf.AppendLine("Missing (" + missing.Count + "):");
                missing.Strings().Indent("  ").Foreach(buf.AppendLine);
            }

            Assert.Fail(buf.ToString());
        }
    }

    /// <summary>Enumerates all the methods tagged with the TestTag attribute.</summary>
    /// <remarks>
    /// This method with the other "tag" methods (e.g. ConstTags) is designed to automatically
    /// check for new constants, function, or whatever which are added or removed from the slate.
    /// This helps ensure that unit-tests are written for all nodes defaulted in the slate.
    /// </remarks>
    /// <param name="type">The type of the class to get tags from.</param>
    /// <returns>All tag values from the given class.</returns>
    static public IEnumerable<string> TestTags(S.Type type) {
        foreach (MethodInfo info in type.GetMethods()) {
            TestTagAttribute? tag = info.GetCustomAttribute<TestTagAttribute>();
            if (tag is not null) yield return tag.Value;
        }
    }

    /// <summary>
    /// Enumerates all the constants reachable from the given
    /// namespace group and returns string tags for them.
    /// </summary>
    /// <param name="group">The namespace to look for constants within.</param>
    /// <param name="prefix">Any prefix namespace to add to the tags.</param>
    /// <returns>All tags for constants reachable from the given namespace.</returns>
    static internal IEnumerable<string> ConstTags(Namespace group, string prefix = "") {
        foreach (KeyValuePair<string, INode> pair in group.Fields) {
            if (pair.Value is IConstant)
                yield return prefix+pair.Key;
            else if (pair.Value is Namespace inner) {
                foreach (string tag in ConstTags(inner, prefix+pair.Key+"."))
                    yield return tag;
            }
        }
    }

    /// <summary>
    /// Enumerates all the function definitions reachable from the given
    /// namespace group and returns string tags for them.
    /// </summary>
    /// <param name="group">The namespace to look for function definitions within.</param>
    /// <param name="prefix">Any prefix namespace to add to the tags.</param>
    /// <returns>All tags for function definitions reachable from the given namespace.</returns>
    static internal IEnumerable<string> FuncDefTags(Namespace group, string prefix = "") {
        foreach (KeyValuePair<string, INode> pair in group.Fields) {
            if (pair.Value is IFuncGroup funcGroup) {
                foreach (IFuncDef def in funcGroup.Definitions)
                    yield return prefix+pair.Key+":"+def.ReturnType.FormattedTypeName();
            } else if (pair.Value is IFuncDef funcDef) {
                yield return prefix+pair.Key+":"+funcDef.ReturnType.FormattedTypeName();
            } else if (pair.Value is Namespace inner) {
                foreach (string tag in FuncDefTags(inner, prefix+pair.Key+"."))
                    yield return tag;
            }
        }
    }
}
