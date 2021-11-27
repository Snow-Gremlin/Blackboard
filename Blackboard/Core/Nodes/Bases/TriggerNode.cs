using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>A base node for any trigger node.</summary>
    public abstract class TriggerNode: Evaluable, ITriggerParent, IConstantable {

        /// <summary>Creates a new trigger node.</summary>
        public TriggerNode(bool provoked = false) => this.Provoked = provoked;

        /// <summary>Converts this node to a constant trigger.</summary>
        /// <returns>The constant trigger carrying the provoked condition.</returns>
        public virtual IConstant ToConstant() => this is IConstant c ? c : new ConstTrigger(this.Provoked);

        /// <summary>Indicates if this trigger has been fired during a current evaluation.</summary>
        public bool Provoked { get; protected set; }

        /// <summary>Resets the trigger at the end of the evaluation.</summary>
        public void Reset() => this.Provoked = false;

        /// <summary>
        /// This is called when the trigger is evaluated and updated.
        /// It will determine if the trigger should be provoked.
        /// </summary>
        /// <returns>True if this trigger should be provoked, false if not.</returns>
        abstract protected bool ShouldProvoke();

        /// <summary>Updates the node's provoked state.</summary>
        /// <returns>True indicates that the value has been provoked, false otherwise.</returns>
        protected override bool Evaluate() => this.Provoked = this.ShouldProvoke();
    }
}
