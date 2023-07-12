using Blackboard.Core.Extensions;
using Blackboard.Core.Record;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BlackboardTests.Tools;

/// <summary>These are extensions to Blackboard Action's Result for testing.</summary>
static class ResultExt {

    /// <summary>Checks if the names in the output are what is expected.</summary>
    /// <param name="result">The results to check the names with.</param>
    /// <param name="expNames">The names to check against.</param>
    /// <param name="names">The path to get the names from.</param>
    static public Result CheckNames(this Result result, string expNames, params string[] names) {
        Assert.AreEqual(expNames, result.OutputNames(names).Join(", "), "Checking the names in the output.");
        return result;
    }

    /// <summary>Gets the message for the CheckValues assertions.</summary>
    /// <param name="type">The type of the value being checked.</param>
    /// <param name="name">The name of the variable to get.</param>
    /// <returns>The message to show in the assertion.</returns>
    static private string checkValueMsg(string type, string[] name) =>
        "Checking the " + type + " value of \"" + name.Join(".") + "\".";

    /// <summary>Checks the boolean value of this node.</summary>
    /// <param name="result">This is the parse result to check the value with.</param>
    /// <param name="exp">The expected boolean value.</param>
    /// <param name="name">The name of the variable to get.</param>
    static public Result CheckValue(this Result result, bool exp, params string[] names) {
        Assert.AreEqual(exp, result.GetBool(names), checkValueMsg("bool", names));
        return result;
    }

    /// <summary>Checks the integer value of this node.</summary>
    /// <param name="result">This is the parse result to check the value with.</param>
    /// <param name="exp">The expected integer value.</param>
    /// <param name="name">The name of the variable to get.</param>
    static public Result CheckValue(this Result result, int exp, params string[] names) {
        Assert.AreEqual(exp, result.GetInt(names), checkValueMsg("int", names));
        return result;
    }

    /// <summary>Checks the double value of this node.</summary>
    /// <param name="result">This is the parse result to check the value with.</param>
    /// <param name="exp">The expected double value.</param>
    /// <param name="name">The name of the variable to get.</param>
    static public Result CheckValue(this Result result, double exp, params string[] names) {
        Assert.AreEqual(exp, result.GetDouble(names), checkValueMsg("double", names));
        return result;
    }

    /// <summary>Checks the string value of this node.</summary>
    /// <param name="result">This is the parse result to check the value with.</param>
    /// <param name="exp">The expected string value.</param>
    /// <param name="name">The name of the variable to get.</param>
    static public Result CheckValue(this Result result, string exp, params string[] names) {
        Assert.AreEqual(exp, result.GetString(names), checkValueMsg("string", names));
        return result;
    }

    /// <summary>Checks the string value of this node.</summary>
    /// <param name="result">This is the parse result to check the value with.</param>
    /// <param name="exp">The expected string value.</param>
    /// <param name="name">The name of the variable to get.</param>
    static public Result CheckObject(this Result result, object exp, params string[] names) {
        Assert.AreEqual(exp, result.GetObject(names), checkValueMsg("object", names));
        return result;
    }
}
