using S = System;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an output trigger.</summary>
    public interface ITriggerOutput: IOutput, ITrigger, IChild {

        /// <summary>This event is emitted when the trigger has been provoked.</summary>
        event S.EventHandler OnProvoked;
    }
}
