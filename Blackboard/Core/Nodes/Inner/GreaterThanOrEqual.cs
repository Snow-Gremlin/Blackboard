using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Determines if the two values are greater than or equal.</summary>
/// <typeparam name="T">The type being compared.</typeparam>
sealed public class GreaterThanOrEqual<T> : BinaryValue<T, T, Bool>
    where T : struct, IComparable<T> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((left, right) => new GreaterThanOrEqual<T>(left, right));

    /// <summary>Creates a greater than or equal value node.</summary>
    public GreaterThanOrEqual() { }

    /// <summary>Creates a greater than or equal value node.</summary>
    /// <param name="source1">This is the first parent for the source value.</param>
    /// <param name="source2">This is the second parent for the source value.</param>
    public GreaterThanOrEqual(IValueParent<T>? source1 = null, IValueParent<T>? source2 = null) :
        base(source1, source2) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new GreaterThanOrEqual<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(GreaterThanOrEqual<T>);

    /// <summary>Determine if the parent's values are greater than or equal during evaluation.</summary>
    /// <param name="value1">The first parent's value to compare.</param>
    /// <param name="value2">The second parent's value to compare.</param>
    /// <returns>True if the first value is greater than or equal than the second value, false otherwise.</returns>
    protected override Bool OnEval(T value1, T value2) => new(value1.CompareTo(value2) >= 0);
}
