using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This is a node for a ternary selection between two values.</summary>
    sealed public class SelectValue<T>: Select<IValueParent<T>>, IValueParent<T>
        where T : IData {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((test, left, right) => new SelectValue<T>(test, left, right));

        /// <summary>Creates a selection value node.</summary>
        public SelectValue() { }

        /// <summary>Creates a selection value node.</summary>
        /// <param name="test">This is the first parent for the boolean for selection between the other two parents.</param>
        /// <param name="left">This is the second parent to select when the test boolean is true.</param>
        /// <param name="right">This is the third parent to select when the test boolean is false.</param>
        public SelectValue(IValueParent<Bool> test = null, IValueParent<T> left = null, IValueParent<T> right = null) :
            base(test, left, right) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new SelectValue<T>();

        /// <summary>The value of this node.</summary>
        public T Value { get; private set; }

        /// <summary>This gets the data being stored in this node.</summary>
        public IData Data => this.Value;

        /// <summary>Updates the node's value, provoked state, and any other state.</summary>
        /// <returns>True indicates that the selected value has changed, false otherwise.</returns>
        public override bool Evaluate() {
            base.Evaluate();
            // It doesn't really matter to the value if this switches the selected node.
            // Even if the selection changes, if the value doesn't change we don't want to indicate a change.

            T value = this.Selected is null ? default : this.Selected.Value;
            if (this.Value.Equals(value)) return false;
            this.Value = value;
            return true;
        }
    }
}
