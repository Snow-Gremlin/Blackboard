using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Formula.Actions;

/// <summary>The interface for all the typed and trigger getter actions.</summary>
internal interface IGetter : IAction {

    /// <summary>The names in the path to write the value to.</summary>
    public string[] Names { get; }

    /// <summary>The node to get the data/trigger to get.</summary>
    public INode Node { get; }

    /// <summary>All the nodes which are new children of the node to write.</summary>
    public EvalPending NeedPending { get; }
}
