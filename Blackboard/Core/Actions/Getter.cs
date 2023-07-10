using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Actions;

/// <summary>This is an action that will get a value to the result.</summary>
/// <typeparam name="T">The type of data for the gotten value.</typeparam>
sealed public class Getter<T> : IGetter
    where T : IData {

    /// <summary>
    /// Creates a getter from the given nodes after first checking
    /// that the nodes can be used in this type of getter.
    /// </summary>
    /// <remarks>It is assumed that these values have been run through the optimizer and validated.</remarks>
    /// <param name="name">The name to write the value to.</param>
    /// <param name="value">The value to get to the given target.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the value.</param>
    /// <returns>The getter action or null if the value can not be gotten.</returns>
    static public Getter<T>? Create(string name, INode value, IEnumerable<INode> allNewNodes) =>
        (value is IValue<T> data) ? new Getter<T>(name, data, allNewNodes) : null;

    /// <summary>The data node to get the data from.</summary>
    private readonly IValue<T> value;

    /// <summary>
    /// This is a subset of all the node for the value which need to be pended
    /// for evaluation in order to perform this get.
    /// </summary>
    private readonly IEvaluable[] needPending;

    /// <summary>Creates a new getter.</summary>
    /// <remarks>It is assumed that these values have been run through the optimizer and validated.</remarks>
    /// <param name="name">The name to get to.</param>
    /// <param name="value">The node to get the value from.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the value.</param>
    public Getter(string name, IValue<T> value, IEnumerable<INode> allNewNodes) {
        this.Name  = name;
        this.value = value;

        // Pre-sort the evaluable nodes.
        LinkedList<IEvaluable> nodes = new();
        nodes.SortInsertUnique(allNewNodes.NotNull().OfType<IEvaluable>());
        this.needPending = nodes.ToArray();
    }

    /// <summary>The name to write the value to.</summary>
    public string Name { get; }

    /// <summary>The data node to get the data to get.</summary>
    public IDataNode Value => this.value;

    /// <summary>All the nodes which are new children of the node to write.</summary>
    public IReadOnlyList<IEvaluable> NeedPending => this.needPending;

    /// <summary>This will perform the action.</summary>
    /// <param name="slate">The slate for this action.</param>
    /// <param name="result">The result being created and added to.</param>
    /// <param name="logger">The optional logger to debug with.</param>
    public void Perform(Slate slate, Record.Result result, Logger? logger = null) {
        logger.Info("Getter: {0}", this);
        slate.PendEval(this.needPending);
        slate.PerformEvaluation(logger);
        result.SetValue(this.Name, this.value.Value);
        logger.Info("Getter Done {0}", this.Name);
    }

    /// <summary>Gets a human readable string for this getter.</summary>
    /// <returns>The human readable string for debugging.</returns>
    public override string ToString() => Stringifier.Simple(this);
}
