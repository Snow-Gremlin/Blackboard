using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Binary nodes for floating point mathematics.</summary>
/// <typeparam name="T">The type of floating point to perform the math on.</typeparam>
static internal class BinaryFloatingPoint<T>
    where T : struct, IFloatingPoint<T>, IEquatable<T> {

    /// <summary>This is a factory for creating a new Round instance of this node.</summary>
    /// <remarks>This returns the rounded value from the parent at the given decimal point.</remarks>
    static public readonly IFuncDef Round = BinaryFunc<T, Int, T>.Factory(nameof(Round), (v1, v2) => v1.Round(v2));

    /// <summary>This is a factory for creating a new Atan2 instance of this node.</summary>
    /// <remarks>
    /// This returns the angle whose tangent is the quotient of two specified numbers
    /// where the first parent is the y value and the second parent is the x value.
    /// </remarks>
    static public readonly IFuncDef Atan2 = factory(nameof(Atan2), S.Math.Atan2);

    /// <summary>This is a factory for creating a new IEEERemainder instance of this node.</summary>
    /// <remarks>
    /// This returns the remainder resulting from the division of a specified number by another
    /// specified number. The first parent is the dividend and the second parent is the divisor.
    /// This is different from a modulo because it uses the IEEE 754 standard remainder algorithm.
    /// </remarks>
    static public readonly IFuncDef IEEERemainder = factory(nameof(IEEERemainder), S.Math.IEEERemainder);

    /// <summary>This is a factory for creating a new Log instance of this node.</summary>
    /// <remarks>This returns the log of the first parent with the base of the second parent.</remarks>
    static public readonly IFuncDef Log = factory(nameof(Log), S.Math.Log);

    /// <summary>This is a factory for creating a new Pow instance of this node.</summary>
    /// <remarks>This returns the first parent raised to the power of the second parent.</remarks>
    static public readonly IFuncDef Pow = factory(nameof(Pow), S.Math.Pow);

    /// <summary>This is a generic factory for creating a new floating point math node around double math function.</summary>
    /// <param name="name">The name of the node this factory creates.</param>
    /// <param name="func">The double math function to perform for this node.</param>
    /// <returns>The new factory function to create instances of this node.</returns>
    static private IFuncDef factory(string name, S.Func<double, double, double> func) =>
        BinaryFunc<T, T, T>.Factory(name, (v1, v2) => v1.DoubleMath(v2, func));
}

/// <summary>Binary nodes for specific hard-coded data types.</summary>
static internal class Binary {

    /// <summary>This is a factory for creating a new PadLeft instance of this node.</summary>
    /// <remarks>
    /// This will return a string padded to the left side with spaces
    /// until the result string is at minimum the given total width.
    /// </remarks>
    static public readonly IFuncDef PadLeft = BinaryFunc<String, Int, String>.
        Factory(nameof(PadLeft), (value, totalWidth) =>
            new(value.Value.PadString(totalWidth.Value, " ", true)));

    /// <summary>This is a factory for creating a new PadRight instance of this node.</summary>
    /// <remarks>
    /// This will return a string padded to the right side with spaces
    /// until the result string is at minimum the given total width.
    /// </remarks>
    static public readonly IFuncDef PadRight = BinaryFunc<String, Int, String>.
        Factory(nameof(PadRight), (value, totalWidth) =>
            new(value.Value.PadString(totalWidth.Value, " ", false)));

    /// <summary>This is a factory for creating a new StartsWith instance of this node.</summary>
    /// <remarks>Determines if a string starts with another string.</remarks>
    static public readonly IFuncDef StartsWith = BinaryFunc<String, String, Bool>.
        Factory(nameof(StartsWith), (v1, v2) =>
            new Bool(v1.Value.StartsWith(v2.Value)));

    /// <summary>This is a factory for creating a new EndsWith instance of this node.</summary>
    /// <remarks>Determines if a string ends with another string.</remarks>
    static public readonly IFuncDef EndsWith = BinaryFunc<String, String, Bool>.
        Factory(nameof(EndsWith), (v1, v2) =>
            new Bool(v1.Value.EndsWith(v2.Value)));

    /// <summary>This is a factory for creating a new Contains instance of this node.</summary>
    /// <remarks>Determines if a string contains with another string.</remarks>
    static public readonly IFuncDef Contains = BinaryFunc<String, String, Bool>.
        Factory(nameof(Contains), (v1, v2) =>
            new Bool(v1.Value.Contains(v2.Value)));

    /// <summary>This is a factory for creating a new IndexOf instance of this node.</summary>
    /// <remarks>Determines the index of a string within another string or -1 if not found.</remarks>
    static public readonly IFuncDef IndexOf = BinaryFunc<String, String, Int>.
        Factory(nameof(IndexOf), (v1, v2) =>
            new Int(v1.Value.IndexOf(v2.Value)));

    /// <summary>This is a factory for creating a new Trim instance of this node.</summary>
    /// <remarks>Trims the given string with the characters from the given string or whitespace.</remarks>
    static public readonly IFuncDef Trim = BinaryFunc<String, String, String>.
        Factory(nameof(Trim), (v1, v2) =>
            new String(v1.Value.Trim(v2.Value.ToCharArray())));

    /// <summary>This is a factory for creating a new TrimStart instance of this node.</summary>
    /// <remarks>Trims the start of the given string with the characters from the given string or whitespace.</remarks>
    static public readonly IFuncDef TrimStart = BinaryFunc<String, String, String>.
        Factory(nameof(TrimStart), (v1, v2) =>
            new String(v1.Value.TrimStart(v2.Value.ToCharArray())));

    /// <summary>This is a factory for creating a new TrimEnd instance of this node.</summary>
    /// <remarks>Trims the end of the given string with the characters from the given string or whitespace.</remarks>
    static public readonly IFuncDef TrimEnd = BinaryFunc<String, String, String>.
        Factory(nameof(TrimEnd), (v1, v2) =>
            new String(v1.Value.TrimEnd(v2.Value.ToCharArray())));
}

/// <summary>This gets the binary mathematical function value from two parents.</summary>
/// <remarks>
/// This uses a little more computation time and more memory that hard coded nodes,
/// therefor this should be used to perform less commonly used nodes.
/// </remarks>
sealed internal class BinaryFunc<T1, T2, TResult> : BinaryValue<T1, T2, TResult>
    where T1 : struct, IEquatable<T1>
    where T2 : struct, IEquatable<T2>
    where TResult : struct, IEquatable<TResult> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    /// <param name="funcName">The display name for this function.</param>
    /// <param name="func">The function to perform for this node.</param>
    static public IFuncDef Factory(string funcName, S.Func<T1, T2, TResult> func) =>
        CreateFactory((value1, value2) => new BinaryFunc<T1, T2, TResult>(funcName, func, value1, value2));

    /// <summary>The name of the function for this mathematics.</summary>
    private readonly string funcName;

    /// <summary>The function to perform on this node's value.</summary>
    private readonly S.Func<T1, T2, TResult> func;

    /// <summary>Creates a binary mathematical function value node.</summary>
    /// <param name="funcName">The name of the function to perform.</param>
    /// <param name="func">This is the function to apply to the parents.</param>
    /// <param name="source1">This is the first parent for the source value.</param>
    /// <param name="source2">This is the second parent for the source value.</param>
    public BinaryFunc(string funcName, S.Func<T1, T2, TResult> func,
        IValueParent<T1>? source1 = null, IValueParent<T2>? source2 = null) :
        base(source1, source2) {
        this.funcName = funcName;
        this.func = func;
    }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new BinaryFunc<T1, T2, TResult>(this.funcName, this.func);

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => this.funcName;

    /// <summary>The result of the mathematical function the parents' value during evaluation.</summary>
    /// <param name="value1">The first value to evaluate.</param>
    /// <param name="value2">The second value to evaluate.</param>
    /// <returns>The new data with the value.</returns>
    protected override TResult OnEval(T1 value1, T2 value2) =>
        this.func is null ? default : this.func(value1, value2);
}
