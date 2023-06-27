using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>An external node as a placeholder for a trigger node.</summary>
sealed public class ExternTrigger : TriggerNode, ITriggerExtern {

    /// <summary>Creates a new extern trigger.</summary>
    public ExternTrigger() { }

    /// <summary>Creates a new instance of this node with similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new ExternTrigger();

    /// <summary>The extern trigger will never provoke.</summary>
    /// <returns>Always returns false.</returns>
    protected override bool ShouldProvoke() => false;

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Extern";
}
