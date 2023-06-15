using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>This is a trigger which will be provoked when all of its non-null parents are provoked.</summary>
sealed public class All : NaryTrigger {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((values) => new All(values));

    /// <summary>Creates an all trigger node.</summary>
    public All() { }

    /// <summary>Creates an all trigger node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public All(params ITriggerParent[] parents) : base(parents) { }

    /// <summary>Creates an all trigger node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public All(IEnumerable<ITriggerParent> parents) : base(parents) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new All();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(All);

    /// <summary>Checks if all of the parents are provoked during evaluation.</summary>
    /// <param name="provoked">The provoked values from the parents.</param>
    /// <returns>True if all the parents are provoked, false otherwise.</returns>
    protected override bool OnEval(IEnumerable<bool> provoked) => provoked.All(p => p);
}
