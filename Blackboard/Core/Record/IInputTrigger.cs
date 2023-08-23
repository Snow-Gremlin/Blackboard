namespace Blackboard.Core.Record;

/// <summary>A trigger which can be provoked.</summary>
public interface IInputTrigger {

    /// <summary>Provokes this trigger.</summary>
    public void Provoke();
}
