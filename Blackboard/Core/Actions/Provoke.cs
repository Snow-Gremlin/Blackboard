﻿using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Actions {

    /// <summary>This is an action that will provoke an input trigger with an optional conditional trigger.</summary>
    sealed public class Provoke: IAction {

        /// <summary>
        /// Creates a conditional provoke from the given nodes after
        /// first checking that the nodes can be used in a provoke.
        /// </summary>
        /// <param name="loc">The location that this provoke was created.</param>
        /// <param name="target">The target node to provoke.</param>
        /// <param name="value">The value to use as the conditional.</param>v
        /// <returns>The provoke action.</returns>
        static public Provoke Create(Location loc, INode target, INode value, IEnumerable<INode> allNodes = null) =>
            (target is ITriggerInput input) && (value is ITrigger conditional) ? new Provoke(input, conditional, allNodes) :
            throw new Exception("Unexpected node types for a conditional provoke.").
                With("Location", loc).
                With("Target",   target).
                With("Value",    value);

        /// <summary>
        /// Creates an unconditional provoke from the given nodes after
        /// first checking that the node can be provoked.
        /// </summary>
        /// <param name="loc">The location that this provoke was created.</param>
        /// <param name="target">The target node to provoke.</param>
        /// <param name="allNodes">All the nodes which are new children of the node to provoke.</param>
        /// <returns>The provoke action.</returns>
        static public Provoke Create(Location loc, INode target, IEnumerable<INode> allNodes = null) =>
            (target is ITriggerInput input) ? new Provoke(input, null, allNodes) :
            throw new Exception("Unexpected node types for a unconditional provoke.").
                With("Location", loc).
                With("Target",   target);

        /// <summary>
        /// This is a subset of all the node for the trigger which need to be pended
        /// for evaluation in order to perform this assignment.
        /// </summary>
        private readonly IEvaluable[] needPending;

        /// <summary>Creates a new provoke action.</summary>
        /// <param name="target">The input trigger to provoke.</param>
        /// <param name="trigger">The optional trigger to conditionally provoke with or null to always provoke.</param>
        /// <param name="allNodes">All the nodes which are new children of the node to provoke.</param>
        public Provoke(ITriggerInput target, ITrigger trigger, IEnumerable<INode> allNodes = null) {
            this.Target  = target;
            this.Trigger = trigger;
            this.needPending = allNodes is null ? S.Array.Empty<IEvaluable>() :
                allNodes.NotNull().OfType<IEvaluable>().ToArray();
        }

        /// <summary>The target input trigger to provoke.</summary>
        public readonly ITriggerInput Target;

        /// <summary>The optional trigger to conditionally provoke with.</summary>
        public readonly ITrigger Trigger;

        /// <summary>All the nodes which are new children of the node to provoke.</summary>
        public IReadOnlyList<IEvaluable> NeedPending => this.needPending;

        /// <summary>This will perform the action.</summary>
        /// <param name="slate">The slate for this action.</param>
        /// <param name="result">The result being created and added to.</param>
        /// <param name="logger">The optional logger to debug with.</param>
        public void Perform(Slate slate, Result result, ILogger logger = null) {
            logger?.Log("Provoke: {0}", this);
            slate.PendEval(this.needPending);
            slate.PerformEvaluation(logger?.Sub);
            slate.Provoke(this.Target, this.Trigger?.Provoked ?? true);
        }

        /// <summary>Gets a human readable string for this provoke.</summary>
        /// <returns>The human readable string for debugging.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
