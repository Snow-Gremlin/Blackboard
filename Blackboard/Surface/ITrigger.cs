namespace Blackboard.Surface;

/// <summary>The interface for an output trigger.</summary>
public interface ITrigger {

    /// <summary>This event is emitted when the trigger has been provoked.</summary>
    event System.EventHandler OnProvoked;
}
