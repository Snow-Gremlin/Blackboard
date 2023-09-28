using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Formula.Actions;

/// <summary>This is an action that will get a value to the result.</summary>
/// <typeparam name="T">The type of data for the gotten value.</typeparam>
sealed internal class ValueGetter<T> : IGetter
    where T : IData {

    /// <summary>
    /// Creates a getter from the given nodes after first checking
    /// that the nodes can be used in this type of getter.
    /// </summary>
    /// <remarks>It is assumed that these nodes have been run through the optimizer and validated.</remarks>
    /// <param name="names">The name in the path to write the value to.</param>
    /// <param name="node">The node to get to the given target.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the node.</param>
    /// <returns>The getter action or null if the node can not be gotten.</returns>
    static public ValueGetter<T>? Create(string[] names, INode node, IEnumerable<INode> allNewNodes) =>
        node is IValue<T> data ? new ValueGetter<T>(names, data, allNewNodes) : null;

    /// <summary>The data node to get the data from.</summary>
    private readonly IValue<T> value;

    /// <summary>Creates a new getter.</summary>
    /// <remarks>It is assumed that these nodes have been run through the optimizer and validated.</remarks>
    /// <param name="names">The name in the path to write the value to.</param>
    /// <param name="node">The node to get the value from.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the node.</param>
    public ValueGetter(string[] names, IValue<T> node, IEnumerable<INode> allNewNodes) {
        this.Names = names;
        this.value = node;

        // Pre-group the evaluable nodes.
        this.NeedPending = new();
        this.NeedPending.Insert(allNewNodes.NotNull().OfType<IEvaluable>());
    }

    /// <summary>The names in the path to write the value to.</summary>
    public string[] Names { get; }

    /// <summary>The data node to the data to get.</summary>
    public INode Node => this.value;

    /// <summary>All the nodes which are new children of the node to write.</summary>
    public EvalPending NeedPending { get; }

    /// <summary>This will perform the action.</summary>
    /// <param name="slate">The slate for this action.</param>
    /// <param name="result">The result being created and added to.</param>
    /// <param name="logger">The optional logger to debug with.</param>
    public void Perform(Slate slate, Record.Result result, Logger? logger = null) {
        logger.Info("Value Getter: {0}", this);
        slate.PendEval(this.NeedPending);
        slate.PerformEvaluation(logger);
        result.SetValue(this.value.Value, this.Names);
        logger.Info("Value Getter Done {0}", this.Names.Join("."));
    }

    /// <summary>Gets a human readable string for this getter.</summary>
    /// <returns>The human readable string for debugging.</returns>
    public override string ToString() => Stringifier.Simple(this);
}
