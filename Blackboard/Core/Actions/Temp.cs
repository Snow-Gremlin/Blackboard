using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Actions;

/// <summary>
/// This is an action to update a temporary node.
/// The temporary node will only evaluate but not written to the slate.
/// </summary>
sealed public class Temp : IAction {

    /// <summary>
    /// This is a subset of all the node for this node to write which need to be
    /// added to parents their parents to make this node reactive to changes.
    /// </summary>
    private readonly IChild[] needParents;

    /// <summary>Creates a new temp action.</summary>
    /// <remarks>It is assumed that these values have been run through the optimizer and validated.</remarks>
    /// <param name="name">The name to write the node with.</param>
    /// <param name="node">The temporary node with the given name.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the node to write.</param>
    public Temp(string name, INode node, IEnumerable<INode>? allNewNodes = null) {
        this.Name = name;
        this.Node = node;
        this.needParents = (allNewNodes ?? Enumerable.Empty<INode>()).Illegitimates().ToArray();
    }

    /// <summary>The name to write the node with.</summary>
    public readonly string Name;

    /// <summary>The temporary node with the given name.</summary>
    public readonly INode Node;

    /// <summary>All the nodes which are new children of the node to write.</summary>
    public IReadOnlyList<IChild> NeedParents => this.needParents;

    /// <summary>This will perform the action.</summary>
    /// <remarks>It is assumed that these values have been run through the optimizer and validated.</remarks>
    /// <param name="slate">The slate for this action.</param>
    /// <param name="result">The result being created and added to.</param>
    /// <param name="logger">The optional logger to debug with.</param>
    public void Perform(Slate slate, Record.Result result, Logger? logger = null) {
        logger.Info("Temp: {0}", this);
        List<IChild> changed = this.needParents.Where(child => child.Legitimatize()).ToList();
        slate.PendUpdate(changed);
        slate.PendEval(changed);
    }

    /// <summary>Gets a human readable string for this temp.</summary>
    /// <returns>The human readable string for debugging.</returns>
    public override string ToString() => Stringifier.Simple(this);
}
