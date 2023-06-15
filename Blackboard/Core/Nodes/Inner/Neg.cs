using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Negates the given parents value.</summary>
sealed public class Neg<T> : UnaryValue<T, T>
    where T : struct, ISigned<T>, IEquatable<T> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((input) => new Neg<T>(input));

    /// <summary>Creates a negated value node.</summary>
    public Neg() { }

    /// <summary>Creates a negated value node.</summary>
    /// <param name="source">This is the single parent for the source value.</param>
    public Neg(IValueParent<T>? source) : base(source) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Neg<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Neg<T>);

    /// <summary>Gets the negated value of the parent during evaluation.</summary>
    /// <param name="value">The parent value to negate.</param>
    /// <returns>The negated parent value.</returns>
    protected override T OnEval(T value) => value.Neg();
}
