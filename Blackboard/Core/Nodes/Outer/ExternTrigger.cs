using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>An external node as a placeholder for a trigger node.</summary>
sealed public class ExternTrigger : UnaryTrigger, ITriggerExtern {

    /// <summary>Creates a new extern trigger.</summary>
    public ExternTrigger() { }

    /// <summary>Creates a new instance of this node with similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new ExternTrigger();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => "Extern"; // So that it is Extern<bool> and Extern<trigger>.
}
