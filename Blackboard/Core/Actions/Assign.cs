using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Debug;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Core.Actions {

    /// <summary>This is an action that will assign an input node to a data.</summary>
    /// <typeparam name="T">The type of data for the input node.</typeparam>
    sealed public class Assign<T>: IAssign
        where T : IData {

        /// <summary>
        /// Creates an assignment from the given nodes after first checking
        /// that the nodes can be used in this type of assignment.
        /// </summary>
        /// <param name="loc">The location that this provoke was created.</param>
        /// <param name="target">The target node to assign to.</param>
        /// <param name="value">The value to assign to the given target.</param>
        /// <returns>The assignment action.</returns>
        static public Assign<T> Create(Location loc, INode target, INode value) =>
            (target is IValueInput<T> input) && (value is IValue<T> data) ?
            new Assign<T>(input, data) :
            throw new Exception("Unexpected node types for assignment.").
                With("Location", loc).
                With("Type", typeof(T)).
                With("Target", target).
                With("Value", value);

        /// <summary>The target input node to set the value of.</summary>
        private readonly IValueInput<T> target;

        /// <summary>The data node to get the data to assign.</summary>
        private readonly IValue<T> value;

        /// <summary>Creates a new assignment.</summary>
        /// <param name="target">The input node to assign.</param>
        /// <param name="value">The node to get the value from.</param>
        public Assign(IValueInput<T> target, IValue<T> value) {
            this.target = target;
            this.value  = value;
        }

        /// <summary>The target input node to set the value of.</summary>
        public IInput Target => this.target;

        /// <summary>The data node to get the data to assign.</summary>
        public IDataNode Value => this.value;

        /// <summary>This will perform the action.</summary>
        /// <param name="driver">The driver for this action.</param>
        public void Perform(Driver driver) => driver.SetValue(this.value.Value, this.target);

        /// <summary>Gets a human readable string for this assignment.</summary>
        /// <returns>The human readable string for debugging.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
