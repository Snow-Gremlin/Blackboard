using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>A shell node as a proxy for a trigger node.</summary>
/// <remarks>
/// A shell should be used in only specific cases, such as when an input or extern
/// is used in a define by itself but we don't want the new define to be input or extern
/// just a node. This prevents a define from making a named copy which cannot be
/// updated with an extern being defined or to be assigned as a define.
/// </remarks>
sealed public class ShellTrigger : UnaryTrigger, ITrigger {
    
    /// <summary>Creates a new shell trigger.</summary>
    public ShellTrigger() { }
    
    /// <summary>Creates a new shell trigger.</summary>
    /// <param name="source">The parent node initialized for this external.</param>
    public ShellTrigger(ITriggerParent source) : base(source) { }

    /// <summary>Creates a new instance of this node with similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new ShellTrigger();

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Shell";
}
