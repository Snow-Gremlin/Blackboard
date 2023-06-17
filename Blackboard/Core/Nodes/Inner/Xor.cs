using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Performs a boolean Exclusive OR of the boolean parents.</summary>
/// <see cref="https://mathworld.wolfram.com/XOR.html"/>
sealed public class Xor : NaryValue<Bool, Bool> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory(inputs => new Xor(inputs));

    /// <summary>Creates a boolean Exclusive OR value node.</summary>
    public Xor() { }

    /// <summary>Creates a boolean Exclusive OR value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public Xor(params IValueParent<Bool>[] parents) : base(parents) { }

    /// <summary>Creates a boolean Exclusive OR value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public Xor(IEnumerable<IValueParent<Bool>> parents) : base(parents) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Xor();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Xor);

    /// <summary>Gets the Exclusive OR of all the parent's booleans.</summary>
    /// <param name="values">The parents to Exclusive OR together.</param>
    /// <returns>The Exclusive OR of all the given values.</returns>
    protected override Bool OnEval(IEnumerable<Bool> values) =>
        new(values.Select((b) => b.Value).Aggregate((left, right) => left ^ right));

    /// <summary>
    /// The identity element for the node which is a constant
    /// to use when coalescing the node for optimization.
    /// </summary>
    public override IConstant Identity => Bool.False.ToLiteral();
}
