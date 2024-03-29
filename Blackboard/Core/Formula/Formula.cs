﻿using Blackboard.Core.Extensions;
using Blackboard.Core.Formula.Actions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Inspect.Loggers;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Formula;

/// <summary>This is a collection of actions pending to perform on the Blackboard slate.</summary>
sealed public class Formula {

    /// <summary>Joins one or more formulas to run together.</summary>
    /// <remarks>This will not evaluate or reset triggers until after the last formula is run.</remarks>
    /// <param name="formulas">The formulas to join together.</param>
    /// <returns>The joined formula to run several formulas together.</returns>
    static public Formula Join(params Formula[] formulas) {
        if (formulas.Length <= 0)
            throw new BlackboardException("Join must have at least one formula in it.");
        Slate slate = formulas[0].slate;
        List<IAction> actions = new();
        foreach (Formula f in formulas) {
            if (f.slate != slate)
                throw new BlackboardException("May only join formulas from the same blackboard.");
            actions.AddRange(f.Actions.Where(action => action is not Finish));
        }
        actions.Add(new Finish());
        return new Formula(slate, actions);
    }

    /// <summary>The collection of performers for this formula.</summary>
    private readonly IAction[] actions;

    /// <summary>Creates a new formula to perform changes to the slate.</summary>
    /// <param name="slate">The slate that these actions were created for.</param>
    /// <param name="actions">The actions that this formula will perform.</param>
    internal Formula(Slate slate, params IAction[] actions) :
        this(slate, actions as IEnumerable<IAction>) { }

    /// <summary>Creates a new formula to perform changes to the slate.</summary>
    /// <param name="slate">The slate that these actions were created for.</param>
    /// <param name="actions">The actions that this formula will perform.</param>
    internal Formula(Slate slate, IEnumerable<IAction> actions) {
        this.actions = actions.NotNull().ToArray();
        this.slate   = slate;
    }

    /// <summary>The slate that this formula was built for and will be run on.</summary>
    internal readonly Slate slate;

    /// <summary>The actions for this formula.</summary>
    internal IReadOnlyList<IAction> Actions => this.actions;

    /// <summary>Performs all the actions for this formula.</summary>
    /// <param name="logger">The optional logger to debug with.</param>
    /// <returns>The results of the formula being performed.</returns>
    public Record.Result Perform(Logger? logger = null) {
        logger.Info("Formula");
        Logger? sub = logger.Group(nameof(Formula));
        Record.Result result = new();
        foreach (IAction action in this.actions)
            action.Perform(this.slate, result, sub);
        return result;
    }

    /// <summary>Gets a human readable string for this formula as all internal actions on different lines.</summary>
    /// <returns>The human readable string for debugging.</returns>
    public override string ToString() => Stringifier.Simple(this);
}
