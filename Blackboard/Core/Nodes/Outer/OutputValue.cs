using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>A node for listening for changes in values used for outputting to the user.</summary>
    /// <typeparam name="T">The type of the value to hold.</typeparam>
    sealed public class OutputValue<T>: UnaryValue<T, T>, IValueOutput<T>
        where T : IComparable<T> {

        /// <summary>Creates a new output value node.</summary>
        public OutputValue() { }

        /// <summary>Creates a new output value node.</summary>
        /// <param name="source">The initial source to get the value from.</param>
        public OutputValue(IValueParent<T> source = null) : base(source) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new OutputValue<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Output";

        /// <summary>This event is emitted when the value is changed.</summary>
        public event S.EventHandler<OutputValueEventArgs<T>> OnChanged;

        /// <summary>Gets the negated value of the parent during evaluation.</summary>
        /// <param name="value">The parent value to negate.</param>
        /// <returns>The negated parent value.</returns>
        protected override T OnEval(T value) =>
            this.Parent is null ? default : this.Parent.Value;

        /// <summary>Updates the node's provoked state.</summary>
        /// <returns>True indicates that the value has changed, false otherwise.</returns>
        public override bool Evaluate() {
            T prev = this.Value;
            if (!base.Evaluate()) return false;
            this.OnChanged?.Invoke(this, new OutputValueEventArgs<T>(prev, this.Value));
            return true;
        }
    }
}
