﻿using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Inspect;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Formula.Actions;

/// <summary>This is an action that will assign an input node to a data.</summary>
/// <typeparam name="T">The type of data for the input node.</typeparam>
sealed internal class Assign<T> : IAssign
    where T : IData {

    /// <summary>
    /// Creates an assignment from the given nodes after first checking
    /// that the nodes can be used in this type of assignment.
    /// </summary>
    /// <remarks>It is assumed that these values have been run through the optimizer and validated.</remarks>
    /// <param name="target">The target node to assign to.</param>
    /// <param name="value">The value to assign to the given target.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the value.</param>
    /// <returns>The assignment action or null if the value cannot be assigned to the target.</returns>
    static public Assign<T>? Create(INode target, INode value, IEnumerable<INode> allNewNodes) =>
        target is IValueInput<T> input && value is IValue<T> data ?
        new Assign<T>(input, data, allNewNodes) : null;

    /// <summary>The target input node to set the value of.</summary>
    private readonly IValueInput<T> target;

    /// <summary>The data node to get the data to assign.</summary>
    private readonly IValue<T> value;

    /// <summary>Creates a new assignment.</summary>
    /// <remarks>It is assumed that these values have been run through the optimizer and validated.</remarks>
    /// <param name="target">The input node to assign.</param>
    /// <param name="value">The node to get the value from.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the value.</param>
    public Assign(IValueInput<T> target, IValue<T> value, IEnumerable<INode> allNewNodes) {
        this.target = target;
        this.value  = value;

        // Pre-group the evaluable nodes.
        this.NeedPending = new();
        this.NeedPending.Insert(allNewNodes.Illegitimates().OfType<IEvaluable>());
    }

    /// <summary>The target input node to set the value of.</summary>
    public IInput Target => this.target;

    /// <summary>The data node to get the data to assign.</summary>
    public IDataNode Value => this.value;

    /// <summary>All the nodes which are new children of the node to write.</summary>
    public EvalPending NeedPending { get; }

    /// <summary>This will perform the action.</summary>
    /// <param name="slate">The slate for this action.</param>
    /// <param name="result">The result being created and added to.</param>
    /// <param name="logger">The optional logger to debug with.</param>
    public void Perform(Slate slate, Record.Result result, Logger? logger = null) {
        logger.Info("Assign: {0}", this);
        slate.PendEval(this.NeedPending);
        slate.PerformEvaluation(logger);
        slate.SetValue(this.value.Value, this.target);
        logger.Info("Assign Done {0}", this.target);
    }

    /// <summary>Gets a human readable string for this assignment.</summary>
    /// <returns>The human readable string for debugging.</returns>
    public override string ToString() => Stringifier.Simple(this);
}
