using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>
/// This will return true when the value is or goes over the maximum but
/// will not return false until the value is or goes below the minimum.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Zener_diode"/>
sealed internal class Zener<T> : TernaryValue<T, T, T, Bool>
    where T : struct, IComparable<T> {

    // TODO: Think about allowing Zener be assignable so that it can be set true or false while within the range.
    // TODO: While we are at making the Zener assignable, does it have to be a custom type like Latch, or can Latch not be a type?

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((value, min, max) => new Zener<T>(value, min, max));

    /// <summary>Creates a Zener ranged value node.</summary>
    public Zener() { }

    /// <summary>Creates a Zener ranged value node.</summary>
    /// <param name="value">This is the value parent that is checked against the range.</param>
    /// <param name="min">This is the minimum value parent for the lower edge of the range.</param>
    /// <param name="max">This is the maximum value parent for the upper edge of the range.</param>
    public Zener(IValueParent<T>? value = null, IValueParent<T>? min = null, IValueParent<T>? max = null) :
        base(value, min, max) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Zener<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Zener<T>);

    /// <summary>Selects the value to return during evaluation.</summary>
    /// <param name="value">
    /// The value from the first parent. This is the input value to test.
    /// If less than or equal to the min value the result will be set to false,
    /// if greater than or equal to the max value the result will be set to true,
    /// if within the range, the value will remain what it was at.
    /// </param>
    /// <param name="min">The minimum value to return.</param>
    /// <param name="max">The maximum value to return.</param>
    /// <returns>True if in the inclusive range and false otherwise.</returns>
    protected override Bool OnEval(T value, T min, T max) =>
        value.CompareTo(min) <= 0 ? Bool.False :
        value.CompareTo(max) >= 0 ? Bool.True :
        this.Value;
}
