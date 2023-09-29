using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Performs a left shifts the first parent the amount of the second parent.</summary>
sealed internal class LeftShift<T> : BinaryValue<T, Int, T>
    where T : struct, IBitwise<T>, IEquatable<T> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((left, right) => new LeftShift<T>(left, right));

    /// <summary>Creates a left shift value node.</summary>
    public LeftShift() { }

    /// <summary>Creates a left shift value node.</summary>
    /// <param name="left">This is the first parent for the source value.</param>
    /// <param name="right">This is the second parent for the source value.</param>
    public LeftShift(IValueParent<T>? left = null, IValueParent<Int>? right = null) : base(left, right) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new LeftShift<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(LeftShift<T>);

    /// <summary>Left shifts the value during evaluation.</summary>
    /// <param name="left">The value to left shift.</param>
    /// <param name="right">The value to left shift the other value by.</param>
    /// <returns>The left shifted value for this node.</returns>
    protected override T OnEval(T left, Int right) => left.LeftShift(right);
}
