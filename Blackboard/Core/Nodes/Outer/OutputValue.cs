using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Surface;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>A node for listening for changes in values used for outputting to the user.</summary>
/// <typeparam name="T">The type of the value to hold.</typeparam>
sealed public class OutputValue<T1, T2> : UnaryValue<T1, T1>, IValueOutput<T1>, Surface.IValue<T2>
    where T1 : struct, IEquatable<T1>, IBaseValue<T2> {

    /// <summary>Creates a new output value node.</summary>
    public OutputValue() { }

    /// <summary>Creates a new output value node.</summary>
    /// <param name="source">The initial source to get the value from.</param>
    public OutputValue(IValueParent<T1>? source = null) : base(source) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new OutputValue<T1, T2>();

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Output";

    /// <summary>This event is emitted when the value is changed.</summary>
    public event System.EventHandler<ValueEventArgs<T2>>? OnChanged;

    /// <summary>Gets the value of the parent during evaluation.</summary>
    /// <param name="value">The parent value to output.</param>
    /// <returns>The parent's value.</returns>
    protected override T1 OnEval(T1 value) => value;

    /// <summary>Updates the node's provoked state.</summary>
    /// <returns>True indicates that the value has changed, false otherwise.</returns>
    public override bool Evaluate() {
        T2 prev = this.Value.BaseValue;
        if (!base.Evaluate()) return false;

        T2 cur = this.Value.BaseValue;
        this.OnChanged?.Invoke(this, new ValueEventArgs<T2>(prev, cur));
        return true;
    }
}
