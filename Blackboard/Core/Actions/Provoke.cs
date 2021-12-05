using Blackboard.Core.Debug;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Core.Actions {

    /// <summary>This is an action that will provoke an input trigger with an optional conditional trigger.</summary>
    sealed public class Provoke: IAction {

        /// <summary>
        /// Creates a conditional provoke from the given nodes after
        /// first checking that the nodes can be used in a provoke.
        /// </summary>
        /// <param name="loc">The location that this provoke was created.</param>
        /// <param name="target">The target node to provoke.</param>
        /// <param name="value">The value to use as the conditional.</param>
        /// <returns>The provoke action.</returns>
        static public Provoke Create(Location loc, INode target, INode value) =>
            (target is ITriggerInput input) && (value is ITrigger conditional) ? new Provoke(input, conditional) :
            throw new Exception("Unexpected node types for a conditional provoke.").
                With("Location", loc).
                With("Target", target).
                With("Value", value);

        /// <summary>
        /// Creates an unconditional provoke from the given nodes after
        /// first checking that the node can be provoked.
        /// </summary>
        /// <param name="loc">The location that this provoke was created.</param>
        /// <param name="target">The target node to provoke.</param>
        /// <returns>The provoke action.</returns>
        static public Provoke Create(Location loc, INode target) =>
            (target is ITriggerInput input) ? new Provoke(input) :
            throw new Exception("Unexpected node types for a unconditional provoke.").
                With("Location", loc).
                With("Target", target);

        /// <summary>Creates a new provoke action.</summary>
        /// <param name="target">The input trigger to provoke.</param>
        /// <param name="trigger">The optional trigger to conditionally provoke with or null to always provoke.</param>
        public Provoke(ITriggerInput target, ITrigger trigger = null) {
            this.Target  = target;
            this.Trigger = trigger;
        }

        /// <summary>The target input trigger to provoke.</summary>
        public readonly ITriggerInput Target;

        /// <summary>The optional trigger to conditionally provoke with.</summary>
        public readonly ITrigger Trigger;

        /// <summary>This will perform the action.</summary>
        /// <param name="driver">The driver for this action.</param>
        public void Perform(Driver driver) =>
            driver.Provoke(this.Target, this.Trigger?.Provoked ?? true);

        /// <summary>Gets a human readable string for this provoke.</summary>
        /// <returns>The human readable string for debugging.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
