using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>This is a node for a ternary selection between two triggers.</summary>
sealed public class SelectTrigger : Select<ITriggerParent>, ITriggerParent {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((test, left, right) => new SelectTrigger(test, left, right));

    /// <summary>Creates a selection trigger node.</summary>
    public SelectTrigger() { }

    /// <summary>Creates a selection trigger node.</summary>
    /// <param name="test">This is the first parent for the boolean for selection between the other two parents.</param>
    /// <param name="left">This is the second parent to select when the test boolean is true.</param>
    /// <param name="right">This is the third parent to select when the test boolean is false.</param>
    public SelectTrigger(IValueParent<Bool>? test = null, ITriggerParent? left = null, ITriggerParent? right = null) :
        base(test, left, right) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new SelectTrigger();

    /// <summary>Indicates this trigger had been provoked during the current evaluation.</summary>
    public bool Provoked { get; private set; }

    /// <summary>Resets the trigger after evaluation has finished.</summary>
    public void Reset() => this.Provoked = false;

    /// <summary>Updates the node's value, provoked state, and any other state.</summary>
    /// <remarks>Here we want to return if provoked and NOT if the provoke state has changed.</remarks>
    /// <returns>True indicates that the selected value has changed, false otherwise.</returns>
    public override bool Evaluate() {
        base.Evaluate();
        // It doesn't really matter to the value if this switches the selected node.
        // Even if the selection changes, if the state isn't provoked we don't want to indicate a change.

        return this.Provoked = this.Selected is not null && this.Selected.Provoked;
    }
}
