namespace Blackboard.Core.Actions {

    /// <summary>This represents an action which can be performed as part of a formula.</summary>
    public interface IAction {

        /// <summary>This will perform the action.</summary>
        /// <param name="driver">The driver for this action.</param>
        public void Perform(Driver driver);
    }
}
