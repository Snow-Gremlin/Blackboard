﻿using Blackboard.Core.Extensions;
using Blackboard.Core.Formula.Actions;
using Blackboard.Core.Inspect.Loggers;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Formula.Factory;

/// <summary>The collection of actions which have been parsed.</summary>
sealed internal class ActionCollection {
    private readonly Slate slate;
    private readonly Logger? logger;
    private readonly LinkedList<IAction> actions;

    /// <summary>Creates new a new action collection.</summary>
    /// <param name="slate">The slate to create the formula for.</param>
    /// <param name="logger">The optional logger to write debugging information to.</param>
    internal ActionCollection(Slate slate, Logger? logger = null) {
        this.slate   = slate;
        this.logger  = logger;
        this.actions = new LinkedList<IAction>();
    }

    /// <summary>Clears the collection of actions.</summary>
    public void Clear() => this.actions.Clear();

    /// <summary>Gets the formula containing all the actions.</summary>
    public Formula CreateFormula() {
        Formula formula = new(this.slate, this.actions.Append(new Finish()));
        this.actions.Clear();
        return formula;
    }

    /// <summary>Adds a pending action into this formula.</summary>
    /// <param name="performer">The performer to add.</param>
    public void Add(IAction action) {
        this.logger.Info("Add Action: {0}", action);
        this.actions.AddLast(action);
    }

    /// <summary>Gets the human readable string of the current actions.</summary>
    /// <returns>The human readable string.</returns>
    public override string ToString() => this.ToString("");

    /// <summary>Gets the human readable string of the current actions.</summary>
    /// <param name="indent">The indent to apply to all but the first line being returned.</param>
    /// <returns>The human readable string.</returns>
    public string ToString(string indent) =>
        this.actions.Count <= 0 ? "[]" :
        "[\n" + indent + this.actions.Strings().Indent(indent).Join(",\n" + indent) + "]";
}
