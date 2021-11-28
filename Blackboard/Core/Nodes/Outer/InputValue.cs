using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>A node for user inputted values.</summary>
    /// <typeparam name="T">The type of the value to hold.</typeparam>
    sealed public class InputValue<T>: ValueNode<T>, IValueInput<T>
        where T : IData, IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = new Function<InputValue<T>>(() => new InputValue<T>());

        /// <summary>
        /// This is a factory function for creating new instances
        /// of this node easily with an initial value from the given node.
        /// </summary>
        static public readonly IFuncDef FactoryWithInitialValue =
            new Function<IValue<T>, InputValue<T>>((IValue<T> node) => new InputValue<T>(node.Value));

        /// <summary>
        /// This is a function to assign a value to the input node.
        /// This will return the input node on success, or null if value could not be cast.
        /// </summary>
        static public readonly IFuncDef Assign =
            new Function<InputValue<T>, IDataNode, InputValue<T>>((InputValue<T> input, IDataNode node) => {
                T value = node.Data.ImplicitCastTo<T>();
                if (value is null) return null;
                input.SetValue(value);
                return input;
            });

        /// <summary>Creates a new input value node.</summary>
        /// <param name="value">The initial value for this node.</param>
        public InputValue(T value = default) : base(value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Input";

        /// <summary>This sets the value of this node.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.UpdateValue(value);

        /// <summary>This is called when the value is evaluated and updated.</summary>
        /// <remarks>
        /// Since the value is set by the user this will always return the current value.
        /// This node typically won't be evaluated. When the value is set, if the value changes,
        /// then the driver should touch the children so that they will be evaluated.
        /// </remarks>
        /// <returns>This will always return the current value.</returns>
        protected override T CalcuateValue() => this.Value;
    }
}
