using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S = System;

namespace BlackboardTests {

    /// <summary>A collection of testing tools.</summary>
    static public class TestTools {

        /// <summary>Check an exception from the given action.</summary>
        /// <param name="hndl">The action to perform.</param>
        /// <param name="exp">The lines of the expected exception message.</param>
        static public void CheckException(S.Action hndl, params string[] exp) {
            Exception ex = Assert.ThrowsException<Exception>(hndl);
            NoDiff(exp, ex.Message.Split("\n"));
        }

        /// <summary>Asserts that all the given lines are equal, otherwise shows the diff.</summary>
        /// <param name="exp">The expected result to check against.</param>
        /// <param name="results">The result that is being checked.</param>
        /// <param name="message">The message to go along with the test.</param>
        static public void NoDiff(string exp, string results, string message = "") =>
            NoDiff(exp.Split("\n"), results.Split("\n"), message);

        /// <summary>Asserts that all the given lines are equal, otherwise shows the diff.</summary>
        /// <param name="exp">The expected result to check against.</param>
        /// <param name="results">The result that is being checked.</param>
        /// <param name="message">The message to go along with the test.</param>
        static public void NoDiff(IEnumerable<string> exp, IEnumerable<string> results, string message = "") {
            string[] expLines = exp.Split("\n").ToArray();
            string[] gotLines = results.Split("\n").ToArray();
            if (!Enumerable.SequenceEqual(expLines, gotLines)) {
                StringBuilder buf = new();
                if (!string.IsNullOrEmpty(message)) {
                    buf.AppendLine(message);
                    buf.AppendLine();
                }

                buf.AppendLine("Diff:");
                gotLines.Diff(expLines).Indent("  ").Foreach(buf.AppendLine);
                buf.AppendLine();

                buf.AppendLine("Expected:");
                expLines.Indent("  ").Foreach(buf.AppendLine);
                buf.AppendLine();

                buf.AppendLine("Results:");
                gotLines.Indent("  ").Foreach(buf.AppendLine);
                buf.AppendLine();

                Assert.Fail(buf.ToString());
            }
        }
    }
}
