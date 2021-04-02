using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using System.Linq;

namespace Blackboard.Core.Bases {

    /// <summary>A base node for any trigger node.</summary>
    public abstract class TriggerNode: Node, ITrigger {

        /// <summary>Creates a new trigger node.</summary>
        protected TriggerNode() {
            this.Triggered = false;
        }

        /// <summary>Indicates if this trigger has been fired during a current evaluation.</summary>
        public bool Triggered { get; protected set; }

        /// <summary>Resets the trigger at the end of the evaluation.</summary>
        public void Reset() => this.Triggered = false;

        /// <summary>This is called when the trigger is evaluated.</summary>
        /// <returns>True if this trigger got triggered, false if not.</returns>
        abstract protected bool UpdateTrigger();

        /// <summary>This evaluates this trigger node.</summary>
        /// <returns>This will always return all the children if triggered, or none if not triggered.</returns>
        sealed public override IEnumerable<INode> Eval() {
            this.Triggered = this.UpdateTrigger();
            return this.Triggered ? this.Children : Enumerable.Empty<INode>();
        }
    }
}
