namespace Blackboard.Core.Record;

/// <summary>The interface for an output trigger.</summary>
public interface ITriggerWatcher {

    /// <summary>This event is emitted when the trigger has been provoked.</summary>
    public event System.EventHandler OnProvoked;
}
