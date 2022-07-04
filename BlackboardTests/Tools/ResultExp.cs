using Blackboard.Core.Actions;
using Blackboard.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BlackboardTests.Tools {

    /// <summary>These are extensions to Blackboard Action's Result for testing.</summary>
    static class ResultExp {

        /// <summary>Checks if the names in the output are what is expected.</summary>
        /// <param name="result">The results to check the names with.</param>
        /// <param name="names">The names to check against.</param>
        static public Result CheckNames(this Result result, string names) {
            Assert.AreEqual(names, result.OutputNames.Join(", "), "Checking the names in the output.");
            return result;
        }

        /// <summary>Gets the message for the CheckValues assertions.</summary>
        /// <param name="type">The type of the value being checked.</param>
        /// <param name="name">The name of the variable to get.</param>
        /// <returns>The message to show in the assertion.</returns>
        static private string checkValueMsg(string type, string name) =>
            "Checking the " + type + " value of \"" + name + "\".";

        /// <summary>Checks the boolean value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected boolean value.</param>
        /// <param name="name">The name of the variable to get.</param>
        static public Result CheckValue(this Result result, bool exp, string name) {
            Assert.AreEqual(exp, result.GetBool(name), checkValueMsg("bool", name));
            return result;
        }

        /// <summary>Checks the integer value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected integer value.</param>
        /// <param name="name">The name of the variable to get.</param>
        static public Result CheckValue(this Result result, int exp, string name) {
            Assert.AreEqual(exp, result.GetInt(name), checkValueMsg("int", name));
            return result;
        }

        /// <summary>Checks the double value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected double value.</param>
        /// <param name="name">The name of the variable to get.</param>
        static public Result CheckValue(this Result result, double exp, string name) {
            Assert.AreEqual(exp, result.GetDouble(name), checkValueMsg("double", name));
            return result;
        }

        /// <summary>Checks the string value of this node.</summary>
        /// <param name="slate">This is the slate to check the value with.</param>
        /// <param name="exp">The expected string value.</param>
        /// <param name="name">The name of the variable to get.</param>
        static public Result CheckValue(this Result result, string exp, string name) {
            Assert.AreEqual(exp, result.GetString(name), checkValueMsg("string", name));
            return result;
        }
    }
}
