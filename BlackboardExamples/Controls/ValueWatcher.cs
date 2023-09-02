using Blackboard.Core.Record;

namespace BlackboardExamples.Controls;

// TODO: Comment
sealed public class ValueWatcher<T>: IBlackBoardComponent {
    private readonly string    name;
    private readonly Action<T> setter;
    private readonly Func<T>   getter;
    private IValueWatcher<T>?  watcher;
    private T prior;

    public ValueWatcher(string name, Action<T> setter, Func<T> getter) {
        this.name    = name;
        this.setter  = setter;
        this.getter  = getter;
        this.watcher = null;
        this.prior   = this.getter();
    }

    public void Connect(Blackboard.Blackboard b, string id) {
        if (this.watcher is not null) this.Disconnect();
        this.prior   = this.getter();
        this.watcher = b.OnChange<T>(id+"."+name);
        this.watcher.OnChanged += this.onChanged;
    }

    public void Disconnect() {
        if (this.watcher is not null) {
            this.watcher.OnChanged -= this.onChanged;
            this.watcher = null;
            this.setter(this.prior);
        }
    }

    private void onChanged(object? sender, ValueEventArgs<T> e) => this.setter(e.Current);
}
