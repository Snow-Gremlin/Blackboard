using Blackboard.Core.Inspect;

namespace Blackboard.Core.Actions {

    /// <summary>This is an action for resetting the provoked triggers.</summary>
    sealed public class ResetTriggers: IAction {

        /// <summary>Creates a new reset trigger action.</summary>
        public ResetTriggers() { }

        /// <summary>This will perform the action.</summary>
        /// <param name="slate">The slate for this action.</param>
        /// <param name="logger">The optional logger to debug with.</param>
        public void Perform(Slate slate, Logger logger = null) => slate.ResetTriggers();

        /// <summary>Gets a human readable string for this reset.</summary>
        /// <returns>The human readable string for debugging.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
