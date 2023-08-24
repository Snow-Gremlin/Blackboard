using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Record;

/// <summary>An input for setting a value.</summary>
/// <typeparam name="T">The C# type of the value to set.</typeparam>
sealed public class InputValue<T>: IInputValue<T> {
    private readonly Slate slate;
    private readonly IInputValue<T> node;

    /// <summary>Creates a new input value.</summary>
    /// <param name="slate">The slate to update when the value is changed.</param>
    /// <param name="node">The node to update the value in.</param>
    internal InputValue(Slate slate, IInputValue<T> node) {
        this.slate = slate;
        this.node  = node;
    }
    
    /// <summary>This sets the value of this node.</summary>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value has changed, false otherwise.</returns>
    public bool SetValue(T value) {
        if (!this.node.SetValue(value)) return false;
        if (this.node is IParent parent)
            this.slate.PendEval(parent.Children);
        this.slate.FinishEvaluation();
        return true;
    }
}
