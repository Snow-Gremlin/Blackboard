using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>This is a trigger which can be provoked from user output.</summary>
sealed internal class OutputTrigger : UnaryTrigger, ITriggerOutput {

    /// <summary>Creates a new output trigger.</summary>
    public OutputTrigger() => this.Pending = false;

    /// <summary>Creates a new output trigger.</summary>
    /// <param name="source">The initial source trigger to listen to.</param>
    public OutputTrigger(ITriggerParent? source = null) : base(source) => this.Pending = false;

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new OutputTrigger();

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Output";

    /// <summary>Indicates this output has pending provoke needs to be emitted.</summary>
    public bool Pending { get; private set; }

    /// <summary>Emits any pending provoke changes for this output.</summary>
    public void Emit() {
        if (!this.Pending) return;
        this.Pending = false;
        this.OnProvoked?.Invoke(this, System.EventArgs.Empty);
    }

    /// <summary>This event is emitted when the trigger has been provoked.</summary>
    public event System.EventHandler? OnProvoked;

    /// <summary>Updates the node's provoked state.</summary>
    /// <remarks>Here we want to return if provoked and NOT if the provoke state has changed.</remarks>
    /// <returns>True indicates that the value has been provoked, false otherwise.</returns>
    public override bool Evaluate() {
        if (!base.Evaluate()) return false;
        this.Pending = this.Provoked;
        return true;
    }
}
