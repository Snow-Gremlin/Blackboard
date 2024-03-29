﻿using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Performs a boolean OR of all the boolean parents.</summary>
/// <see cref="https://mathworld.wolfram.com/OR.html"/>
sealed internal class Or : NaryValue<Bool, Bool> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((inputs) => new Or(inputs));

    /// <summary>Creates a boolean OR value node.</summary>
    public Or() { }

    /// <summary>Creates a boolean OR value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public Or(params IValueParent<Bool>[] parents) : base(parents) { }

    /// <summary>Creates a boolean OR value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public Or(IEnumerable<IValueParent<Bool>> parents) : base(parents) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Or();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Or);

    /// <summary>Gets the OR of all the parent's booleans.</summary>
    /// <param name="values">The parents to OR together.</param>
    /// <returns>The OR of all the given values.</returns>
    protected override Bool OnEval(IEnumerable<Bool> values) => new(values.Any(val => val.Value));

    /// <summary>
    /// The identity element for the node which is a constant
    /// to use when coalescing the node for optimization.
    /// </summary>
    public override IConstant Identity => Bool.False.ToLiteral();
}
