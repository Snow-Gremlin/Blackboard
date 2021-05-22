using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Provides a node which can be used to count trigger events.</summary>
    sealed public class Counter: ValueNode<int> {

        /// <summary>This is the parent to increment the counter.</summary>
        private ITrigger increment;

        /// <summary>This is the parent to decrement the counter.</summary>
        private ITrigger decrement;

        /// <summary>This is the parent reset the counter to the reset value.</summary>
        private ITrigger reset;

        /// <summary>The value to step during an increment or decrement.</summary>
        private ValueNode<int> delta;

        /// <summary>The value to reset this toggle to when the toggle is reset.</summary>
        private ValueNode<int> resetValue;

        /// <summary>Creates a new node for counting events.</summary>
        /// <param name="increment">The initial parent to trigger an increment.</param>
        /// <param name="decrement">The initial parent to trigger an decrement.</param>
        /// <param name="reset">The initial parent trigger to reset the counter.</param>
        /// <param name="delta">The initial parent for the step value.</param>
        /// <param name="resetValue">The initial reset value parent.</param>
        /// <param name="value">The initial value for this counter.</param>
        public Counter(ITrigger increment = null, ITrigger decrement = null, ITrigger reset = null,
            ValueNode<int> delta = null, ValueNode<int> resetValue = null, int value = default) : base(value) {
            this.Increment = increment;
            this.Decrement = decrement;
            this.Reset = reset;
            this.Delta = delta;
            this.ResetValue = resetValue;
        }

        /// <summary>This is the parent to increment the counter.</summary>
        public ITrigger Increment {
            get => this.increment;
            set => this.SetParent(ref this.increment, value);
        }

        /// <summary>This is the parent to decrement the counter.</summary>
        public ITrigger Decrement {
            get => this.decrement;
            set => this.SetParent(ref this.decrement, value);
        }

        /// <summary>This is the parent reset the toggle to false.</summary>
        public ITrigger Reset {
            get => this.reset;
            set => this.SetParent(ref this.reset, value);
        }

        /// <summary>The value to step during an increment or decrement.</summary>
        /// <remarks>If this parent is null then the counter will increment and decrement by one.</remarks>
        public ValueNode<int> Delta {
            get => this.delta;
            set => this.SetParent(ref this.delta, value);
        }

        /// <summary>The value to reset this toggle to when the toggle is reset.</summary>
        /// <remarks>If this parent is null then the toggle is reset to false.</remarks>
        public ValueNode<int> ResetValue {
            get => this.resetValue;
            set => this.SetParent(ref this.resetValue, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (!(this.increment is null)) yield return this.increment;
                if (!(this.decrement is null)) yield return this.decrement;
                if (!(this.reset is null)) yield return this.reset;
                if (!(this.delta is null)) yield return this.delta;
                if (!(this.resetValue is null)) yield return this.resetValue;
            }
        }

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override bool UpdateValue() {
            int value = this.Value;
            int delta = this.delta?.Value ?? 1;
            if (this.increment?.Triggered ?? false) value += delta;
            if (this.decrement?.Triggered ?? false) value -= delta;
            if (this.reset?.Triggered ?? false) value = this.resetValue?.Value ?? 0;
            return this.SetNodeValue(value);
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() =>
            "Counter("+NodeString(this.increment)+", "+NodeString(this.decrement)+", "+
                NodeString(this.reset)+", "+NodeString(this.delta)+", "+NodeString(this.resetValue)+")";
    }
}
