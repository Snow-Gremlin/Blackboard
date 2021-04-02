namespace Blackboard.Core.Interfaces {

    /// <summary>The interface for an input trigger.</summary>
    public interface ITriggerInput: ITrigger, IInput {

        /// <summary>Triggers this trigger so that this node is triggered during the next evaluation.</summary>
        void Trigger();
    }
}
