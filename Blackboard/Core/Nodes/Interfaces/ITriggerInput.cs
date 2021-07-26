namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an input trigger.</summary>
    /// <remarks>All inputs may be used as an output.</remarks>
    public interface ITriggerInput: IInput {

        /// <summary>Triggers this trigger so that this node is provoked during the next evaluation.</summary>
        /// <param name="value">True will trigger, false will reset the trigger.</param>
        /// <remarks>This is not intended to be be called directly, it should be called via the driver.</remarks>
        void Trigger(bool value = true);
    }
}
