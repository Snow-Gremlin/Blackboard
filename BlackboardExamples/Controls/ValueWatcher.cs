using Blackboard.Core.Record;

namespace BlackboardExamples.Controls;

/// <summary>
/// A value watcher for listening for value changes in blackboard
/// and updating a control outside of it.
/// </summary>
/// <remarks>
/// This is designed for the example where the control can be connected and disconnected
/// to different blackboards. Normally something like this watcher isn't needed and the
/// controls can use blackboard's `OnChange` directly and easily.
/// </remarks>
/// <typeparam name="T">The C# type to watch for.</typeparam>
sealed public class ValueWatcher<T>: IBlackBoardComponent {
    private readonly string    name;
    private readonly Action<T> setter;
    private readonly Func<T>   getter;
    private IValueWatcher<T>?  watcher;
    private T prior;

    /// <summary>Creates a new watcher.</summary>
    /// <param name="name">The name of the value in blackboard to watch with optional namespaces.</param>
    /// <param name="setter">The setter to set the control's value when the blackboard value changes.</param>
    /// <param name="getter">The getter to get the control's value to use as the default value.</param>
    public ValueWatcher(string name, Action<T> setter, Func<T> getter) {
        this.name    = name;
        this.setter  = setter;
        this.getter  = getter;
        this.watcher = null;
        this.prior   = this.getter();
    }

    /// <summary>Connects this watcher to the given blackboard.</summary>
    /// <remarks>If already connected to a blackboard, this will disconnect from the prior one first.</remarks>
    /// <param name="b">The blackboard to add this watcher to.</param>
    /// <param name="id">The id of the control that works as a prefix namespace for the value name.</param>
    public void Connect(Blackboard.Blackboard b, string id) {
        if (this.watcher is not null) this.Disconnect();
        this.prior   = this.getter();
        this.watcher = b.OnChange(id+"."+this.name, this.prior);
        this.watcher.OnChanged += this.onChanged;
    }

    /// <summary>Disconnects this watcher from the current blackboard.</summary>
    /// <remarks>If not connected this will have no effect.</remarks>
    public void Disconnect() {
        if (this.watcher is not null) {
            this.watcher.OnChanged -= this.onChanged;
            this.watcher = null;
            this.setter(this.prior);
        }
    }

    /// <summary>Handles the change in the blackboard.</summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">The argument for the changed value.</param>
    private void onChanged(object? sender, ValueEventArgs<T> e) => this.setter(e.Current);
}
