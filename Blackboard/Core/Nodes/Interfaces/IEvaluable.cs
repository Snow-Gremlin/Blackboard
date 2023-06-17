namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>An interface for anything evaluable.</summary>
public interface IEvaluable : IParent {

    /// <summary>The depth in the graph from the furthest input of this node.</summary>
    public int Depth { get; set; }

    /// <summary>Updates the node's value, provoked state, and any other state.</summary>
    /// <returns>
    /// True indicates that the value has changed or a trigger has been provoked, false otherwise.
    /// When the value has changed all the children are returned from the evaluation,
    /// otherwise no children are returned.
    /// </returns>
    public bool Evaluate();
}
