using Blackboard.Core.Actions;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using System.Linq;

namespace BlackboardTests.Tools {

    /// <summary>These are extensions to Blackboard Formula for testing.</summary>
    static class FormulaExt {

        /// <summary>Performs the formula and outputs the logs to the console.</summary>
        /// <param name="formula">The formula to perform.</param>
        /// <returns>The result from the perform.</returns>
        static public Result LogPerform(this Formula formula) =>
            formula.Perform(new ConsoleLogger());

        /// <summary>Performs the formula and checks that the log output as expected..</summary>
        /// <param name="formula">The formula to perform.</param>
        /// <param name="lines">The expected evaluation log output.</param>
        /// <returns>The result from the perform.</returns>
        static public Result CheckPerform(this Formula formula, params string[] lines) {
            BufferLogger logger = new();
            Result result = formula.Perform(logger.Stringify(Stringifier.Shallow().PreLoadNames(formula.Slate)));
            TestTools.NoDiff(lines.Join("\n"), logger.ToString().Trim());
            return result;
        }

        /// <summary>Checks the expected string for the given formula.</summary>
        /// <param name="formula">The formula to check the string for.</param>
        /// <param name="lines">The lines for the expected string.</param>
        /// <returns>The formula that is passed in so this can be chained.</returns>
        static public Formula Check(this Formula formula, params string[] lines) {
            TestTools.NoDiff(lines, Stringifier.Deep(formula).Trim().Split("\n"));
            return formula;
        }

        /// <summary>Creates a new formula without the finish action.</summary>
        /// <param name="formula">The formula to copy without the finish action.</param>
        /// <returns>The new formula copy from the given formula but without the finish action.</returns>
        static public Formula NoFinish(this Formula formula) =>
            new(formula.Slate, formula.Actions.Where(action => action is not Finish));
    }
}
