using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Record;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>A node for user inputted values.</summary>
/// <typeparam name="T1">The type of the value to hold.</typeparam>
/// <typeparam name="T2">The C# type behind the blackboard type.</typeparam>
sealed internal class InputValue<T1, T2> : ValueNode<T1>, IValueInput<T1>, IInputValue<T2>
    where T1 : struct, IData, IEquatable<T1>, IBaseValue<T1, T2> {

    /// <summary>Creates a new input value node.</summary>
    public InputValue() { }

    /// <summary>Creates a new input value node.</summary>
    /// <param name="value">The initial value for this node.</param>
    public InputValue(T1 value = default) : base(value) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new InputValue<T1, T2>();

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Input";
    
    /// <summary>Sets the value of this input via data.</summary>
    /// <remarks>This will throw an exception if the data type is not valid for the given input.</remarks>
    /// <param name="data">The data value to assign to the input.</param>
    /// <returns>True if there was any change, false otherwise.</returns>
    public bool SetData(IData data) => data is T1 value ? this.SetValue(value) :
        throw new BlackboardException("Setting the wrong type of data to an input").
            With("data", data).
            With("type", typeof(T1));

    /// <summary>This sets the value of this node.</summary>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value has changed, false otherwise.</returns>
    public bool SetValue(T1 value) => this.UpdateValue(value);

    /// <summary>This sets the value of this node with the given C# value type.</summary>
    /// <param name="baseValue">The value to set.</param>
    /// <returns>True if the value has changed, false otherwise.</returns>
    public bool SetValue(T2 baseValue) => this.SetValue(this.Value.Wrap(baseValue));

    /// <summary>This is called when the value is evaluated and updated.</summary>
    /// <remarks>
    /// Since the value is set by the user this will always return the current value.
    /// This node typically won't be evaluated. When the value is set, if the value changes,
    /// then the slate should pend evaluation for the children so that they will be updated.
    /// </remarks>
    /// <returns>This will always return the current value.</returns>
    protected override T1 CalculateValue() => this.Value;
}
