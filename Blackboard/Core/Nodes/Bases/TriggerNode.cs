using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>A base node for any trigger node.</summary>
    public abstract class TriggerNode: Evaluable, ITriggerParent, IConstantable {

        /// <summary>Creates a new trigger node.</summary>
        public TriggerNode(bool provoked = false) => this.Provoked = provoked;

        /// <summary>Indicates if this trigger has been fired during a current evaluation.</summary>
        public bool Provoked { get; private set; }

        /// <summary>Sets the given provoked state to this node.</summary>
        ///<param name="provoked">The new provoked state to set.</param>
        /// <returns>True if the state has changed, false otherwise.</returns>
        protected bool UpdateProvoked(bool provoked) {
            if (this.Provoked.Equals(provoked)) return false;
            this.Provoked = provoked;
            return true;
        }

        /// <summary>Resets the trigger at the end of the evaluation.</summary>
        public void Reset() => this.Provoked = false;

        /// <summary>
        /// This is called when the trigger is evaluated and updated.
        /// It will determine if the trigger should be provoked.
        /// </summary>
        /// <returns>True if this trigger should be provoked, false if not.</returns>
        abstract protected bool ShouldProvoke();

        /// <summary>Updates the node's provoked state.</summary>
        /// <remarks>Here we want to return if provoked and NOT if the provoke state has changed.</remarks>
        /// <returns>True indicates that the value has been provoked, false otherwise.</returns>
        public override bool Evaluate() => this.Provoked = this.ShouldProvoke();
    }
}
