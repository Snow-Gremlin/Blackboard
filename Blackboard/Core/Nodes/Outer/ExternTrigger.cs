using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>An external node as a placeholder for a trigger node.</summary>
sealed public class ExternTrigger : Evaluable, ITriggerParent, ITriggerExtern {

    /// <summary>Creates a new extern trigger.</summary>
    public ExternTrigger() { }

    /// <summary>Creates a new instance of this node with similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new ExternTrigger();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => "Extern"; // So that it is Extern<bool> and Extern<trigger>.
    
    /// <summary>An external trigger may not be provoked.</summary>
    /// <remarks>The node which replaces this external node defines how provocations occur.</remarks>
    public bool Provoked => false;
    
    /// <summary>Resets the trigger at the end of the evaluation.</summary>
    public void Reset() { }

    /// <summary>Placeholder for the update evaluation that will occur in the replaced node.</summary>
    /// <returns>This will always return false.</returns>
    public override bool Evaluate() => this.Provoked;
}
