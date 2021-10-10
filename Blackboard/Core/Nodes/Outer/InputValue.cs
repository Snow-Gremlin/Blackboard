using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>A node for user inputted values.</summary>
    /// <typeparam name="T">The type of the value to hold.</typeparam>
    sealed public class InputValue<T>: ValueNode<T>, IValueInput<T>
        where T : IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncGroup Factory = new Function<InputValue<T>>(() => new InputValue<T>());

        /// <summary>Creates a new input value node.</summary>
        /// <param name="value">The initial value for this node.</param>
        public InputValue(T value = default) :
            base(value) { }

        /// <summary>Always returns no parents since inputs have no parent.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

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

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Input<" + this.Value + ">";
    }
}
