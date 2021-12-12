using Blackboard.Core.Inspect;

namespace Blackboard.Core.Actions {

    /// <summary>This represents an action which can be performed as part of a formula.</summary>
    public interface IAction {

        /// <summary>This will perform the action.</summary>
        /// <remarks>
        /// The given driver MUST be the driver this action was created for
        /// since several of these actions will hold onto nodes from a specific driver.
        /// </remarks>
        /// <param name="driver">The driver for this action.</param>
        /// <param name="logger">The optional logger to debug with.</param>
        public void Perform(Driver driver, Logger logger = null);
    }
}
