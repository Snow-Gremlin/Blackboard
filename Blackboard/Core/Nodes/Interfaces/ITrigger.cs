namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for a trigger node.</summary>
internal interface ITrigger : INode {

    /// <summary>Indicates this trigger had been provoked during the current evaluation.</summary>
    public bool Provoked { get; }

    /// <summary>Resets the trigger after evaluation has finished.</summary>
    public void Reset();
}
