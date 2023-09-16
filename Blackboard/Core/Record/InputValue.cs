using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Record;

/// <summary>An input for setting a value.</summary>
/// <typeparam name="T">The C# type of the value to set.</typeparam>
sealed public class InputValue<T> {
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
    /// <param name="logger">An optional logger for debugging.</param>
    /// <returns>True if the value has changed, false otherwise.</returns>
    public bool SetValue(T value, Logger? logger = null) {

        logger ??= new ConsoleLogger(); // TODO: REMOVE

        if (!this.node.SetValue(value)) return false;
        logger?.Info("Changed: "+this.node);
        if (this.node is IParent parent)
            this.slate.PendEval(parent.Children);

        // TODO: Should probably make the group suspend finalization also have a
        //       suspend evaluation unless required by something like an assignment or get.

        this.slate.PerformEvaluation(logger);
        this.slate.FinishEvaluation(logger);
        return true;
    }
}
