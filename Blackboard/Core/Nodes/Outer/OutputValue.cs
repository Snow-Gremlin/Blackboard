using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Record;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>A node for listening for changes in values used for outputting to the user.</summary>
/// <typeparam name="T1">The type of the value to hold.</typeparam>
/// <typeparam name="T2">The C# type behind the blackboard type.</typeparam>
sealed internal class OutputValue<T1, T2> : UnaryValue<T1, T1>, IValueOutput<T1>, IValueWatcher<T2>
    where T1 : struct, IEquatable<T1>, IBaseValue<T1, T2> {
    private T2 previous;

    /// <summary>Creates a new output value node.</summary>
    public OutputValue() : this(null) { }

    /// <summary>Creates a new output value node.</summary>
    /// <param name="source">The initial source to get the value from.</param>
    public OutputValue(IValueParent<T1>? source) : base(source) {
        this.Pending  = false;
        this.previous = this.DefaultValue.BaseValue;
    }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new OutputValue<T1, T2>();

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Output";

    /// <summary>Gets the current value.</summary>
    public T2 Current => this.Value.BaseValue;

    /// <summary>Indicates this output has pending values that need to be emitted.</summary>
    public bool Pending { get; private set; }

    /// <summary>Emits any pending value changes for this output.</summary>
    public void Emit() {
        if (!this.Pending) return;
        this.Pending = false;
        T2 prev = this.previous;
        T2 cur = this.Value.BaseValue;
        this.previous = cur;
        this.OnChanged?.Invoke(this, new ValueEventArgs<T2>(prev, cur));
    }

    /// <summary>This event is emitted when the value is changed.</summary>
    public event System.EventHandler<ValueEventArgs<T2>>? OnChanged;

    /// <summary>Gets the value of the parent during evaluation.</summary>
    /// <param name="value">The parent value to output.</param>
    /// <returns>The parent's value.</returns>
    protected override T1 OnEval(T1 value) => value;

    /// <summary>Updates the node's provoked state.</summary>
    /// <returns>True indicates that the value has changed, false otherwise.</returns>
    public override bool Evaluate() {
        if (this.Pending) return base.Evaluate();

        T2 prev = this.Value.BaseValue;
        if (!base.Evaluate()) return false;
        this.Pending = true;
        this.previous = prev;
        return true;
    }
}
