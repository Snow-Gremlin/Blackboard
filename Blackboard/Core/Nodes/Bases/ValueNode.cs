using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>A base node for a node which has a value.</summary>
    /// <typeparam name="T">The type of the value being held.</typeparam>
    public abstract class ValueNode<T>: Evaluable, IValueParent<T>, IConstantable
        where T : IComparable<T> {

        /// <summary>Creates a new value node.</summary>
        /// <param name="value">The initial value of the node.</param>
        public ValueNode(T value = default) => this.Value = value ?? default;

        /// <summary>Converts this node to a constant.</summary>
        /// <returns>A constant of this node.</returns>
        public virtual IConstant ToConstant() => new Literal<T>(this.Value);

        /// <summary>This gets the data being stored in this node.</summary>
        /// <returns>The data being stored.</returns>
        public IData Data => this.Value;

        /// <summary>The value being held by this node.</summary>
        public T Value { get; private set; }

        /// <summary>
        /// This is called when the value is evaluated and updated.
        /// It will determine the new value the node should be set to.
        /// </summary>
        /// <returns>The new value that the node should be set to.</returns>
        abstract protected T CalcuateValue();

        /// <summary>Updates the node's provoked state.</summary>
        /// <returns>True indicates that the value has been provoked, false otherwise.</returns>
        protected override bool Evaluate() {
            T value = this.CalcuateValue();
            if (this.Value.Equals(value)) return false;
            this.Value = value;
            return true;
        }
    }
}
