using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>A node for user inputted values.</summary>
    /// <typeparam name="T">The type of the value to hold.</typeparam>
    sealed public class InputValue<T>: ValueNode<T>, IValueInput<T>
        where T : IData, IComparable<T>, new() {

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
        public InputValue(T value = default) :
            base(value) { }

        /// <summary>Always returns no parents since inputs have no parent.</summary>
        public override IEnumerable<IAdopter> Parents => Enumerable.Empty<IAdopter>();

        /// <summary>This sets the value of this node.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.SetNodeValue(value);

        /// <summary>This will update the value.</summary>
        /// <remarks>
        /// Since the value is set by the user this will always return true.
        /// If the value didn't change during setting then it should not be evaluated.
        /// </remarks>
        /// <returns>This will always return true.</returns>
        protected override bool UpdateValue() => true;

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Input";
    }
}
