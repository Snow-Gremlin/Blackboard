using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>A shell node as a proxy for a trigger node.</summary>
/// <remarks>
/// A shell should be used in only specific cases, such as when an input or extern
/// is used in a define by itself but we don't want the new define to be input or extern
/// just a node. This prevents a define from making a named copy which cannot be
/// updated with an extern being defined or to be assigned as a define.
/// </remarks>
sealed public class ShellTrigger : Evaluable, ITriggerParent, IChild {
    
    /// <summary>The parent source to listen to.</summary>
    private ITriggerParent? source;
    
    /// <summary>Creates a new shell trigger.</summary>
    public ShellTrigger() { }

    /// <summary>Creates a new shell trigger.</summary>
    /// <param name="source">The parent node initialized for this shell.</param>
    public ShellTrigger(ITriggerParent source) => this.source = source;

    /// <summary>Creates a new instance of this node with similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new ShellTrigger();
    
    /// <summary>The parent trigger node to listen to.</summary>
    public ITriggerParent? Parent {
        get => this.source;
        set => IChild.SetParent(this, ref this.source, value);
    }

    /// <summary>The set of parent nodes to this node in the graph.</summary>
    public ParentCollection Parents => new ParentCollection(this, 1).
        With(() => this.source, parent => this.source = parent);

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Shell";

    /// <summary>
    /// Usually resets the trigger at the end of the evaluation
    /// but does nothing for the shell.
    /// </summary>
    public void Reset() { }
    
    /// <summary>Indicates if this trigger has been fired during a current evaluation.</summary>
    public bool Provoked => this.source?.Provoked ?? false;

    /// <summary>Normally updates the node's value, but for the shell this does nothing.</summary>
    /// <returns>Always returns true assuming if evaluated, it's parent has been changed.</returns>
    public override bool Evaluate() => true;
}
