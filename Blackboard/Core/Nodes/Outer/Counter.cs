using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>Provides a node which can be used to count trigger events.</summary>
    sealed public class Counter<T>: ValueNode<T>
        where T : IArithmetic<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        /// <remarks>This will not initialize any sources except increment, the others can be set later.</remarks>
        static public readonly IFuncDef Factory =
            new Function<ITriggerAdopter, Counter<T>>((ITriggerAdopter increment) => new Counter<T>(increment));

        /// <summary>This is the parent to increment the counter.</summary>
        private ITriggerAdopter increment;

        /// <summary>This is the parent to decrement the counter.</summary>
        private ITriggerAdopter decrement;

        /// <summary>This is the parent reset the counter to the reset value.</summary>
        private ITriggerAdopter reset;

        /// <summary>The value to step during an increment or decrement.</summary>
        private IValueAdopter<T> delta;

        /// <summary>The value to reset this toggle to when the toggle is reset.</summary>
        private IValueAdopter<T> resetValue;

        /// <summary>Creates a new node for counting events.</summary>
        /// <param name="increment">The initial parent to trigger an increment.</param>
        /// <param name="decrement">The initial parent to trigger an decrement.</param>
        /// <param name="reset">The initial parent trigger to reset the counter.</param>
        /// <param name="delta">The initial parent for the step value.</param>
        /// <param name="resetValue">The initial reset value parent.</param>
        /// <param name="value">The initial value for this counter.</param>
        public Counter(ITriggerAdopter increment = null, ITriggerAdopter decrement = null, ITriggerAdopter reset = null,
            IValueAdopter<T> delta = null, IValueAdopter<T> resetValue = null, T value = default) : base(value) {
            this.Increment = increment;
            this.Decrement = decrement;
            this.Reset = reset;
            this.Delta = delta;
            this.ResetValue = resetValue;
        }

        /// <summary>This is the parent to increment the counter.</summary>
        public ITriggerAdopter Increment {
            get => this.increment;
            set => this.SetParent(ref this.increment, value);
        }

        /// <summary>This is the parent to decrement the counter.</summary>
        public ITriggerAdopter Decrement {
            get => this.decrement;
            set => this.SetParent(ref this.decrement, value);
        }

        /// <summary>This is the parent reset the toggle to false.</summary>
        public ITriggerAdopter Reset {
            get => this.reset;
            set => this.SetParent(ref this.reset, value);
        }

        /// <summary>The value to step during an increment or decrement.</summary>
        /// <remarks>If this parent is null then the counter will increment and decrement by one.</remarks>
        public IValueAdopter<T> Delta {
            get => this.delta;
            set => this.SetParent(ref this.delta, value);
        }

        /// <summary>The value to reset this toggle to when the toggle is reset.</summary>
        /// <remarks>If this parent is null then the toggle is reset to false.</remarks>
        public IValueAdopter<T> ResetValue {
            get => this.resetValue;
            set => this.SetParent(ref this.resetValue, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.increment  is not null) yield return this.increment;
                if (this.decrement  is not null) yield return this.decrement;
                if (this.reset      is not null) yield return this.reset;
                if (this.delta      is not null) yield return this.delta;
                if (this.resetValue is not null) yield return this.resetValue;
            }
        }

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override bool UpdateValue() {
            T value = this.Value;
            T delta = this.delta is null ? new T().Inc() : this.delta.Value;
            if (this.increment?.Provoked ?? false) value = value.Sum(delta);
            if (this.decrement?.Provoked ?? false) value = value.Sub(delta);
            if (this.reset?.Provoked     ?? false) value = this.resetValue is null ? new() : this.resetValue.Value;
            return this.SetNodeValue(value);
        }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Counter";
    }
}
