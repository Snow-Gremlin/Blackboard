using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>This is an implicit cast from a bool to a trigger.</summary>
/// <remarks>
/// This forces a trigger into an always provoked or not provoked state so that a boolean value can be used with
/// a trigger (e.g. `Trigger && (10 > 3)`). However this trigger will only act provoked, i.e. update its children,
/// when the boolean changes value. This means that you can get an update from this trigger when it is not provoked.
/// </remarks>
sealed public class BoolAsTrigger : TriggerNode, IChild {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory =
            new Function<IValueParent<Bool>, BoolAsTrigger>((value) => new BoolAsTrigger(value));

    /// <summary>This is the parent node to read from.</summary>
    private IValueParent<Bool>? source;

    /// <summary>Creates a new bool value to trigger conversion.</summary>
    public BoolAsTrigger() { }

    /// <summary>Creates a new bool value to trigger conversion.</summary>
    /// <param name="source">The boolean parent to get the provoked state from.</param>
    public BoolAsTrigger(IValueParent<Bool>? source) => this.Parent = source;

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new BoolAsTrigger();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(BoolAsTrigger);

    /// <summary>The parent node to get the source value from.</summary>
    public IValueParent<Bool>? Parent {
        get => this.source;
        set => IChild.SetParent(this, ref this.source, value);
    }

    /// <summary>The set of parent nodes to this node in the graph.</summary>
    public ParentCollection Parents => new ParentCollection(this, 1).
        With(() => this.source, parent => this.source = parent);

    /// <summary>
    /// This is called when the trigger is evaluated and updated.
    /// It will determine if the trigger should be provoked.
    /// </summary>
    /// <returns>True if this trigger should be provoked, false if not.</returns>
    protected override bool ShouldProvoke() => this.source?.Value.Value ?? false;
}
