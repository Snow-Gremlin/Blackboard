using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Performs a boolean AND of all the boolean parents.</summary>
/// <see cref="https://mathworld.wolfram.com/AND.html"/>
sealed internal class And : NaryValue<Bool, Bool> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((values) => new And(values));

    /// <summary>Creates a boolean AND value node.</summary>
    public And() : base() { }

    /// <summary>Creates a boolean AND value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public And(params IValueParent<Bool>[] parents) : base(parents) { }

    /// <summary>Creates a boolean AND value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public And(IEnumerable<IValueParent<Bool>> parents) : base(parents) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new And();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(And);

    /// <summary>Gets the AND of all the parent's booleans.</summary>
    /// <param name="values">The parents to AND together.</param>
    /// <returns>The AND of all the given values.</returns>
    protected override Bool OnEval(IEnumerable<Bool> values) => new(values.All(val => val.Value));

    /// <summary>
    /// The identity element for the node which is a constant
    /// to use when coalescing the node for optimization.
    /// </summary>
    public override IConstant Identity => Literal.Bool(true);
}
