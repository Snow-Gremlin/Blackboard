using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>This will divide the first parent value by the second parent value.</summary>
sealed internal class Div<T> : BinaryValue<T, T, T>
    where T : struct, IDivisible<T>, IEquatable<T> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((dividend, divisor) => new Div<T>(dividend, divisor));

    /// <summary>Creates a divided value node.</summary>
    public Div() { }

    /// <summary>Creates a divided value node.</summary>
    /// <param name="dividend">This is the first parent for the dividend value.</param>
    /// <param name="divisor">This is the second parent for the divisor value.</param>
    public Div(IValueParent<T>? dividend = null, IValueParent<T>? divisor = null) : base(dividend, divisor) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Div<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Div<T>);

    /// <summary>Gets the first value divided by a second value.</summary>
    /// <param name="dividend">The first value to divide.</param>
    /// <param name="divisor">The second value to divide.</param>
    /// <returns>The two values divided, or the default if divide by zero.</returns>
    protected override T OnEval(T dividend, T divisor) => dividend.Div(divisor);
}
