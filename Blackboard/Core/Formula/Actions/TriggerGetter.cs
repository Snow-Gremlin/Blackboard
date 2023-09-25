using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Record;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Formula.Actions;

/// <summary>This is an action that will get a provoke state to the result.</summary>
sealed public class TriggerGetter : IGetter {

    /// <summary>
    /// Creates a getter from the given nodes after first checking
    /// that the nodes can be used in this type of getter.
    /// </summary>
    /// <remarks>It is assumed that these nodes have been run through the optimizer and validated.</remarks>
    /// <param name="names">The name in the path to write the value to.</param>
    /// <param name="node">The node to get to the given target.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the node.</param>
    /// <returns>The getter action or null if the node can not be gotten.</returns>
    static public TriggerGetter? Create(string[] names, INode node, IEnumerable<INode> allNewNodes) =>
        node is ITrigger data ? new TriggerGetter(names, data, allNewNodes) : null;

    /// <summary>The data node to get the data from.</summary>
    private readonly ITrigger trigger;

    /// <summary>Creates a new getter.</summary>
    /// <remarks>It is assumed that these nodes have been run through the optimizer and validated.</remarks>
    /// <param name="names">The name in the path to write the provoke state to.</param>
    /// <param name="node">The node to get the provoke state from.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the node.</param>
    public TriggerGetter(string[] names, ITrigger node, IEnumerable<INode> allNewNodes) {
        this.Names   = names;
        this.trigger = node;

        // Pre-group the evaluable nodes.
        this.NeedPending = new();
        this.NeedPending.Insert(allNewNodes.NotNull().OfType<IEvaluable>());
    }

    /// <summary>The names in the path to write the provoke state to.</summary>
    public string[] Names { get; }

    /// <summary>The trigger node to the provoke state to get.</summary>
    public INode Node => this.trigger;

    /// <summary>All the nodes which are new children of the node to write.</summary>
    public EvalPending NeedPending { get; }

    /// <summary>This will perform the action.</summary>
    /// <param name="slate">The slate for this action.</param>
    /// <param name="result">The result being created and added to.</param>
    /// <param name="logger">The optional logger to debug with.</param>
    public void Perform(Slate slate, Record.Result result, Logger? logger = null) {
        logger.Info("Trigger Getter: {0}", this);
        slate.PendEval(this.NeedPending);
        slate.PerformEvaluation(logger);
        result.SetTrigger(this.trigger.Provoked, this.Names);
        logger.Info("Trigger Getter Done {0}", this.Names.Join("."));
    }

    /// <summary>Gets a human readable string for this getter.</summary>
    /// <returns>The human readable string for debugging.</returns>
    public override string ToString() => Stringifier.Simple(this);
}
