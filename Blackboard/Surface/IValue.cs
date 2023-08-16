namespace Blackboard.Surface;

/// <summary>Event arguments for the change in a value node.</summary>
/// <typeparam name="T">The type of the value node.</typeparam>
sealed public class ValueEventArgs<T> : System.EventArgs {

    /// <summary>The previous value before the change.</summary>
    public readonly T Previous;

    /// <summary>The current value after the change.</summary>
    public readonly T Current;

    /// <summary>Creates a new event arguments for a value node.</summary>
    /// <param name="prev">The previous value before the change.</param>
    /// <param name="cur">The current value after the change.</param>
    public ValueEventArgs(T prev, T cur) {
        this.Previous = prev;
        this.Current = cur;
    }
}

/// <summary>The interface for an output which has a value.</summary>
/// <typeparam name="T">The type of the value to output.</typeparam>
public interface IValue<T> {
    
    /// <summary>This event is emitted when the value is changed.</summary>
    event System.EventHandler<ValueEventArgs<T>> OnChanged;
}
