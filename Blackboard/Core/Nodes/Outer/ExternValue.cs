using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>An external node as a placeholder for value node.</summary>
/// <typeparam name="T">The type of the value to hold.</typeparam>
sealed public class ExternValue<T> : UnaryValue<T, T>, IValueExtern<T>
    where T : struct, IData, IEquatable<T> {

    /// <summary>Creates a new extern value node.</summary>
    public ExternValue() { }

    /// <summary>Creates a new extern value node.</summary>
    /// <param name="value">The default value for this node.</param>
    public ExternValue(T value = default) {
        this.SetValue(value);
    }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new ExternValue<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => "Extern"; // So that it is Extern<bool> and Extern<trigger>.

    /// <summary>Keep the set value the same when the parent is null.</summary>
    protected override T DefaultValue => this.Value;

    /// <summary>If the parent is set, then this will be called, so just return the parent value.</summary>
    /// <param name="value">The value from the parent to pass through.</param>
    /// <returns>The value from the parent unchanged.</returns>
    protected override T OnEval(T value) => value;

    /// <summary>This sets the value of this node.</summary>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value has changed, false otherwise.</returns>
    public bool SetValue(T value) => this.UpdateValue(value);
}
