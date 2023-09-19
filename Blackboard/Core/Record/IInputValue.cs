namespace Blackboard.Core.Record;

/// <summary>Allows for the value of a node to be set.</summary>
/// <typeparam name="T">The C# type of value to set.</typeparam>
public interface IInputValue<T> {

    /// <summary>This sets the value of this node.</summary>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value has changed, false otherwise.</returns>
    public bool SetValue(T value);
}
