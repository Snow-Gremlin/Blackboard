namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an input trigger.</summary>
    public interface ITriggerInput: IInput, ITrigger {

        /// <summary>Provokes this trigger so that this node is provoked during the next evaluation.</summary>
        /// <param name="value">True will provoke, false will reset the trigger.</param>
        /// <remarks>This is not intended to be called directly, it should be called via the driver.</remarks>
        /// <returns>True if there was any change, false otherwise.</returns>
        public bool Provoke(bool value = true);
    }
}
