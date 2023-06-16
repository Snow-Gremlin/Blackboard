using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Ternary nodes for specific hard-coded data types.</summary>
static public class TernaryComparable<T>
    where T : struct, IComparable<T> {

    /// <summary>This is a factory for creating a new InRange instance of this node.</summary>
    /// <remarks>This will determine if the given value is inclusively with a range.</remarks>
    static public readonly IFuncDef InRange = TernaryFunc<T, T, T, Bool>.
        Factory(nameof(InRange), (value, min, max) => new Bool(value.InRange(min, max)));

    /// <summary>This is a factory for creating a new InRange instance of this node.</summary>
    /// <remarks>This will return the value limited to a range.</remarks>
    static public readonly IFuncDef Clamp = TernaryFunc<T, T, T, T>.
        Factory(nameof(Clamp), (value, min, max) => value.Clamp(min, max));
}

/// <summary>Ternary nodes for specific hard-coded data types.</summary>
static public class Ternary {

    /// <summary>This is a factory for creating a new PadLeft instance of this node.</summary>
    /// <remarks>
    /// This will return a string padded to the left side with the given string
    /// until the result string is at minimum the given total width.
    /// </remarks>
    static public readonly IFuncDef PadLeft = TernaryFunc<String, Int, String, String>.
        Factory(nameof(PadLeft), (value, totalWidth, padding) =>
            new(value.Value.PadString(totalWidth.Value, padding.Value, true)));

    /// <summary>This is a factory for creating a new PadRight instance of this node.</summary>
    /// <remarks>
    /// This will return a string padded to the right side with the given string
    /// until the result string is at minimum the given total width.
    /// </remarks>
    static public readonly IFuncDef PadRight = TernaryFunc<String, Int, String, String>.
        Factory(nameof(PadRight), (value, totalWidth, padding) =>
            new(value.Value.PadString(totalWidth.Value, padding.Value, false)));

    /// <summary>This is a factory for creating a new IndexOf instance of this node.</summary>
    /// <remarks>This will return the index of a substring or -1 if not found.</remarks>
    static public readonly IFuncDef IndexOf = TernaryFunc<String, String, Int, Int>.
        Factory(nameof(IndexOf), (value, gram, index) =>
            new(value.Value.IndexOf(gram.Value, index.Value)));

    /// <summary>This is a factory for creating a new Substring instance of this node.</summary>
    /// <remarks>This will return a substring for the given range from a source string.</remarks>
    static public readonly IFuncDef Substring = TernaryFunc<String, Int, Int, String>.
        Factory(nameof(Substring), (value, index, length) =>
            new(value.Value.Substring(index.Value, length.Value)));

    /// <summary>This is a factory for creating a new Remove instance of this node.</summary>
    /// <remarks>This will return a string with the given range removed from it.</remarks>
    static public readonly IFuncDef Remove = TernaryFunc<String, Int, Int, String>.
        Factory(nameof(Remove), (value, index, length) =>
            new(value.Value.Remove(index.Value, length.Value)));

    /// <summary>This is a factory for creating a new Insert instance of this node.</summary>
    /// <remarks>This will return a string with the given new string inserted at the given index.</remarks>
    static public readonly IFuncDef Insert = TernaryFunc<String, Int, String, String>.
        Factory(nameof(Insert), (value, index, gram) =>
            new(value.Value.Insert(index.Value, gram.Value)));
}

/// <summary>This gets the ternary mathematical function value from two parents.</summary>
/// <remarks>
/// This uses a little more computation time and more memory that hard coded nodes,
/// therefor this should be used to perform less commonly used nodes.
/// </remarks>
sealed public class TernaryFunc<T1, T2, T3, TResult> : TernaryValue<T1, T2, T3, TResult>
    where T1 : struct, IEquatable<T1>
    where T2 : struct, IEquatable<T2>
    where T3 : struct, IEquatable<T3>
    where TResult : struct, IEquatable<TResult> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    /// <param name="funcName">The display name for this function.</param>
    /// <param name="func">The function to perform for this node.</param>
    static public IFuncDef Factory(string funcName, S.Func<T1, T2, T3, TResult> func) =>
        CreateFactory((value1, value2, value3) => new TernaryFunc<T1, T2, T3, TResult>(funcName, func, value1, value2, value3));

    /// <summary>The name of the function for this mathematics.</summary>
    private readonly string funcName;

    /// <summary>The function to perform on this node's value.</summary>
    private readonly S.Func<T1, T2, T3, TResult> func;

    /// <summary>Creates a ternary mathematical function value node.</summary>
    /// <param name="funcName">The name of the function to perform.</param>
    /// <param name="func">This is the function to apply to the parents.</param>
    /// <param name="source1">This is the first parent for the source value.</param>
    /// <param name="source2">This is the second parent for the source value.</param>
    public TernaryFunc(string funcName, S.Func<T1, T2, T3, TResult> func,
        IValueParent<T1>? source1 = null, IValueParent<T2>? source2 = null, IValueParent<T3>? source3 = null) :
        base(source1, source2, source3) {
        this.funcName = funcName;
        this.func = func;
    }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new TernaryFunc<T1, T2, T3, TResult>(this.funcName, this.func);

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => this.funcName;

    /// <summary>The result of the mathematical function the parents' value during evaluation.</summary>
    /// <param name="value1">The first value to evaluate.</param>
    /// <param name="value2">The second value to evaluate.</param>
    /// <returns>The new data with the value.</returns>
    protected override TResult OnEval(T1 value1, T2 value2, T3 value3) =>
        this.func is null ? default : this.func(value1, value2, value3);
}
