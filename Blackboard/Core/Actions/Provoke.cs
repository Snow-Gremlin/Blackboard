using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Actions {

    /// <summary>This is an action that will provoke an input trigger with an optional conditional trigger.</summary>
    sealed public class Provoke: IAction {

        /// <summary>The target input trigger to provoke.</summary>
        private readonly ITriggerInput target;

        /// <summary>The optional trigger to conditionally provoke with.</summary>
        private readonly ITrigger trigger;

        /// <summary>Creates a new provoke action.</summary>
        /// <param name="target">The input trigger to provoke.</param>
        /// <param name="trigger">The optional trigger to conditionally provoke with or null to always provoke.</param>
        public Provoke(ITriggerInput target, ITrigger trigger = null) {
            this.target  = target;
            this.trigger = trigger;
        }

        /// <summary>This will perform the action.</summary>
        /// <param name="driver">The driver for this action.</param>
        public void Perform(Driver driver) {
            if (this.target.Provoke(this.trigger?.Provoked ?? true))
                driver.Touch(this.target.Children);
        }
    }
}
