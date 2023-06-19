using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>An external node as a placeholder for value node.</summary>
/// <typeparam name="T">The type of the value to hold.</typeparam>
sealed public class ExternValue<T> : ValueNode<T>, IValueExtern<T>
    where T : struct, IData, IEquatable<T> {

    /// <summary>Creates a new extern value node.</summary>
    public ExternValue() { }

    /// <summary>Creates a new extern value node.</summary>
    /// <param name="value">The default value for this node.</param>
    public ExternValue(T value = default) : base(value) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new ExternValue<T>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => "Extern"; // So that it is Extern<bool> and Extern<trigger>.

    /// <summary>This sets the value of this node.</summary>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value has changed, false otherwise.</returns>
    public bool SetValue(T value) => this.UpdateValue(value);

    /// <summary>This is called when the value is evaluated and updated.</summary>
    /// <remarks>
    /// Since the default value is set once this will always return the current value.
    /// This node typically won't be evaluated. When the value is set, if the value changes,
    /// then the slate should pend evaluation for the children so that they will be updated.
    /// </remarks>
    /// <returns>This will always return the current value.</returns>
    protected override T CalculateValue() => this.Value;
}
