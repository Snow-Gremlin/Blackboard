using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>This will return the linear interpolation between two parent values.</summary>
sealed public class Lerp<T> : TernaryValue<T, T, T, T>
    where T : struct, IFloatingPoint<T>, IEquatable<T> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((value, min, max) => new Lerp<T>(value, min, max));

    /// <summary>Creates a linear interpolation value node.</summary>
    public Lerp() { }

    /// <summary>Creates a linear interpolation value node.</summary>
    /// <param name="value">This is the first parent for the source value.</param>
    /// <param name="min">This is the second parent for the minimum value.</param>
    /// <param name="max">This is the third parent for the maximum value.</param>
    public Lerp(IValueParent<T>? value = null, IValueParent<T>? min = null, IValueParent<T>? max = null) :
        base(value, min, max) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Lerp<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Lerp<T>);

    /// <summary>Selects the value to return during evaluation.</summary>
    /// <param name="value">
    /// The value from the first parent. This is the iterator value.
    /// Zero or less will return the value from the min parent.
    /// One or more will return the value from the max parent.
    /// Between zero and one will return the linear interpolation.
    /// </param>
    /// <param name="min">The minimum value to return.</param>
    /// <param name="max">The maximum value to return.</param>
    /// <returns>The lerp value to set to this node.</returns>
    protected override T OnEval(T value, T min, T max) => value.Lerp(min, max);
}
