namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for a node used for output.</summary>
/// <remarks>
/// Output nodes are attached to a parent node which is the source of the data
/// or trigger that is being outputted. The output nodes watch for changes in the
/// parent and perform some action such as emitting an event on change.
/// This is different from IInput which can also be read from because input nodes
/// can be read from directly without watching for changes.
/// </remarks>
public interface IOutput : IChild {

    /// <summary>Indicates there is an output event waiting to be emitted.</summary>
    bool Pending { get; }

    /// <summary>Emits any pending output.</summary>
    void Emit();
}
