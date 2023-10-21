using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Bases;

/// <summary>This is a trigger which has a single parent as a source.</summary>
internal abstract class UnaryTrigger : TriggerNode, IChild {
    
    /// <summary>This is a helper for creating unary node factories quickly.</summary>
    /// <param name="handle">The handler for calling the node constructor.</param>
    /// <param name="needsOneNoCast">Indicates that at least one argument must not be a cast.</param>
    static public IFuncDef CreateFactory<Tout>(S.Func<ITriggerParent, Tout> handle,
        bool needsOneNoCast = false)
        where Tout : UnaryTrigger =>
        new Function<ITriggerParent, Tout>(handle, needsOneNoCast);

    /// <summary>The parent source to listen to.</summary>
    private ITriggerParent? source;

    /// <summary>Creates a new unary trigger.</summary>
    public UnaryTrigger() { }

    /// <summary>Creates a new unary trigger.</summary>
    /// <param name="source">The initial source trigger to listen to.</param>
    public UnaryTrigger(ITriggerParent? source = null) => this.Parent = source;

    /// <summary>The parent trigger node to listen to.</summary>
    public ITriggerParent? Parent {
        get => this.source;
        set => IChild.SetParent(this, ref this.source, value);
    }

    /// <summary>The set of parent nodes to this node in the graph.</summary>
    public ParentCollection Parents => new ParentCollection(this, 1).
        With(() => this.source, parent => this.source = parent);

    /// <summary>
    /// This handles updating this node's state given the
    /// parent's provoked state during evaluation.
    /// </summary>
    /// <param name="provoked">The state from the parent.</param>
    /// <returns>The new provoke state for this node.</returns>
    protected virtual bool OnEval(bool provoked) => provoked;

    /// <summary>This updates the trigger during the an evaluation.</summary>
    /// <returns>This returns the provoked value as it currently is.</returns>
    protected override bool ShouldProvoke() =>
        this.source is not null && this.OnEval(this.source.Provoked);
}
