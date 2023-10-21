using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Provokes when the XOR of the parents is provoked.</summary>
sealed internal class XorTrigger : NaryTrigger {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory(inputs => new XorTrigger(inputs));

    /// <summary>Creates an XOR trigger node.</summary>
    public XorTrigger() { }

    /// <summary>Creates an XOR trigger node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public XorTrigger(params ITriggerParent[] parents) : base(parents) { }

    /// <summary>Creates an XOR trigger node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public XorTrigger(IEnumerable<ITriggerParent> parents) : base(parents) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new XorTrigger();

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Xor";

    /// <summary>Updates this trigger during evaluation.</summary>
    /// <param name="provoked">The parent triggers to check.</param>
    /// <returns>True if XOR of the parents was provoked.</returns>
    protected override bool OnEval(IEnumerable<bool> provoked) =>
        provoked.Aggregate((left, right) => left ^ right);
}
