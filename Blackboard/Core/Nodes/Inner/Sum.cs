using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Gets the sum of all of the parent values.</summary>
sealed public class Sum<T> : NaryValue<T, T>
    where T : struct, IAdditive<T>, IEquatable<T> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    /// <param name="needsOneNoCast">
    /// Indicates that at least one argument must not be a cast.
    /// This is used for things like String where all types can implicit cast to a string.
    /// </param>
    static public IFuncDef Factory(bool needsOneNoCast = false) =>
        new FunctionN<IValueParent<T>, Sum<T>>((inputs) => new Sum<T>(inputs), needsOneNoCast: needsOneNoCast);

    /// <summary>Creates a sum value node.</summary>
    public Sum() { }

    /// <summary>Creates a sum value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public Sum(params IValueParent<T>[] parents) : base(parents) { }

    /// <summary>Creates a sum value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public Sum(IEnumerable<IValueParent<T>> parents) : base(parents) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Sum<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Sum<T>);

    /// <summary>Gets the sum of all the parent values.</summary>
    /// <param name="values">The values to sum together.</param>
    /// <returns>The sum of the parent values.</returns>
    protected override T OnEval(IEnumerable<T> values) => new T().Sum(values);

    /// <summary>
    /// The identity element for the node which is a constant
    /// to use when coalescing the node for optimization.
    /// </summary>
    public override IConstant Identity => default(T).SumIdentityValue.ToLiteral();

    /// <summary>Indicates that the parents can be reordered.</summary>
    public override bool Commutative => default(T).SumCommutable;
}
