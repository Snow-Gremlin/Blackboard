namespace Blackboard.Core.Record;

/// <summary>A trigger which can be provoked.</summary>
public interface IInputTrigger {

    /// <summary>Provokes this trigger.</summary>
    /// <returns>True if there was any change, false otherwise.</returns>
    public bool Provoke();
}
