using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Performs a bitwise AND of all the integer parents.</summary>
/// <see cref="https://mathworld.wolfram.com/AND.html"/>
sealed public class BitwiseAnd<T> : NaryValue<T, T>
    where T : struct, IBitwise<T>, IEquatable<T> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((values) => new BitwiseAnd<T>(values));

    /// <summary>Creates a bitwise AND value node.</summary>
    public BitwiseAnd() { }

    /// <summary>Creates a bitwise AND value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public BitwiseAnd(params IValueParent<T>[] parents) : base(parents) { }

    /// <summary>Creates a bitwise AND value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public BitwiseAnd(IEnumerable<IValueParent<T>> parents) : base(parents) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new BitwiseAnd<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(BitwiseAnd<T>);

    /// <summary>Gets the bitwise AND of all the parent's booleans.</summary>
    /// <param name="values">The parents to bitwise AND together.</param>
    /// <returns>The bitwise AND of all the given values.</returns>
    protected override T OnEval(IEnumerable<T> values) => new T().BitwiseAnd(values);

    /// <summary>
    /// The identity element for the node which is a constant
    /// to use when coalescing the node for optimization.
    /// </summary>
    public override IConstant Identity => default(T).AndIdentityValue.ToLiteral();
}
