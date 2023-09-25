using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Formula.Actions;

/// <summary>The interface for all the typed assignment actions.</summary>
/// <remarks>
/// This is only implemented by Assign but has no type parameter
/// so that all typed Assigns can easily be used generically.
/// </remarks>
public interface IAssign : IAction {

    /// <summary>The target input node to set the value of.</summary>
    public IInput Target { get; }

    /// <summary>The data node to get the data to assign.</summary>
    public IDataNode Value { get; }

    /// <summary>All the nodes which are new children of the node to write.</summary>
    public EvalPending NeedPending { get; }
}
