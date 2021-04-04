using System;

namespace Blackboard.Core.Interfaces {

    /// <summary>The interface for an output trigger.</summary>
    public interface ITriggerOutput: ITrigger, IOutput {

        /// <summary>This event is emitted when the trigger has been triggered.</summary>
        event EventHandler OnTriggered;
    }
}
