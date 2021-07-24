namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an output trigger.</summary>
    public interface ITriggerOutput: ITrigger, IOutput {

        /// <summary>This event is emitted when the trigger has been provoked.</summary>
        event System.EventHandler OnProvoked;
    }
}
