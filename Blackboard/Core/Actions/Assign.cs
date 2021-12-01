using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Actions {

    /// <summary>This is an action that will assign an input node to a data.</summary>
    /// <typeparam name="T">The type of data for the input node.</typeparam>
    sealed public class Assign<T>: IAction
        where T : IData {

        /// <summary>The target input node to set the value of.</summary>
        private IValueInput<T> target;

        /// <summary>The data node to get the data to assign.</summary>
        private IDataNode value;

        /// <summary>Creates a new assignment.</summary>
        /// <remarks>
        /// Before creating this action ensure that the value type can be
        /// implicitly cast to the targets type.
        /// </remarks>
        /// <param name="target">The input node to assign.</param>
        /// <param name="value">The node to get the value from.</param>
        public Assign(IValueInput<T> target, IDataNode value) {
            this.target = target;
            this.value  = value;
        }

        /// <summary>This will perform the action.</summary>
        /// <param name="driver">The driver for this action.</param>
        public void Perform(Driver driver) {
            T value = this.value.Data.ImplicitCastTo<T>();
            if (value is not null)
                throw new Exception("Failed to cast while performing an assignment action").
                    With("Target", this.target).
                    With("Value", this.value);

            if (this.target.SetValue(value))
                driver.Touch(this.target.Children);
        }
    }
}
