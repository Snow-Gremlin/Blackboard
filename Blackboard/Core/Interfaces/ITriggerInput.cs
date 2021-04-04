namespace Blackboard.Core.Interfaces {

    /// <summary>The interface for an input trigger.</summary>
    /// <remarks>All inputs may be used as an output.</remarks>
    public interface ITriggerInput: ITriggerOutput, IInput {

        /// <summary>Triggers this trigger so that this node is triggered during the next evaluation.</summary>
        void Trigger();
    }
}
