using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Record;

/// <summary>An input for provoking a trigger.</summary>
sealed public class InputTrigger: IInputTrigger {
    private readonly Slate slate;
    private readonly IInputTrigger node;

    /// <summary>Creates a new trigger provoker.</summary>
    /// <param name="slate">The slate to update when the trigger is changed.</param>
    /// <param name="node">The node to update the provoked state in.</param>
    internal InputTrigger(Slate slate, IInputTrigger node) {
        this.slate = slate;
        this.node  = node;
    }

    /// <summary>Provokes this trigger.</summary>
    /// <returns>True if there was any change, false otherwise.</returns>
    public bool Provoke() {
        if (!this.node.Provoke()) return false;
        if (this.node is IParent parent)
            this.slate.PendEval(parent.Children);
        this.slate.FinishEvaluation();
        return true;
    }
}
