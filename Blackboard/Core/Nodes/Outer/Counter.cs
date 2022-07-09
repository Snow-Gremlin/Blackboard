using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>Provides a node which can be used to count trigger events.</summary>
    sealed public class Counter<T> : ValueNode<T>, IValueInput<T>, IChild
        where T : ISubtractive<T>, IAdditive<T>, IFinite<T>, IEquatable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        /// <remarks>This will not initialize any sources except increment, the others can be set later.</remarks>
        static public readonly IFuncDef Factory =
            new Function<ITriggerParent, Counter<T>>((ITriggerParent increment) => new Counter<T>(increment));

        /// <summary>This is the parent to increment the counter.</summary>
        private ITriggerParent increment;

        /// <summary>This is the parent to decrement the counter.</summary>
        private ITriggerParent decrement;

        /// <summary>This is the parent reset the counter to the reset value.</summary>
        private ITriggerParent reset;

        /// <summary>The value to step during an increment or decrement.</summary>
        private IValueParent<T> delta;

        /// <summary>The value to reset this toggle to when the toggle is reset.</summary>
        private IValueParent<T> resetValue;

        /// <summary>Creates a new node for counting events.</summary>
        public Counter() { }

        /// <summary>Creates a new node for counting events.</summary>
        /// <param name="increment">The initial parent to trigger an increment.</param>
        /// <param name="decrement">The initial parent to trigger an decrement.</param>
        /// <param name="reset">The initial parent trigger to reset the counter.</param>
        /// <param name="delta">The initial parent for the step value.</param>
        /// <param name="resetValue">The initial reset value parent.</param>
        /// <param name="value">The initial value for this node.</param>
        public Counter(ITriggerParent increment = null, ITriggerParent decrement = null, ITriggerParent reset = null,
            IValueParent<T> delta = null, IValueParent<T> resetValue = null, T value = default) : base(value) {
            this.Increment = increment;
            this.Decrement = decrement;
            this.Reset = reset;
            this.Delta = delta;
            this.ResetValue = resetValue;
        }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Counter<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Counter<T>);

        /// <summary>This is the parent to increment the counter.</summary>
        public ITriggerParent Increment {
            get => this.increment;
            set => IChild.SetParent(this, ref this.increment, value);
        }

        /// <summary>This is the parent to decrement the counter.</summary>
        public ITriggerParent Decrement {
            get => this.decrement;
            set => IChild.SetParent(this, ref this.decrement, value);
        }

        /// <summary>This is the parent reset the toggle to false.</summary>
        public ITriggerParent Reset {
            get => this.reset;
            set => IChild.SetParent(this, ref this.reset, value);
        }

        /// <summary>The value to step during an increment or decrement.</summary>
        /// <remarks>If this parent is null then the counter will increment and decrement by one.</remarks>
        public IValueParent<T> Delta {
            get => this.delta;
            set => IChild.SetParent(this, ref this.delta, value);
        }

        /// <summary>The value to reset this toggle to when the toggle is reset.</summary>
        /// <remarks>If this parent is null then the toggle is reset to false.</remarks>
        public IValueParent<T> ResetValue {
            get => this.resetValue;
            set => IChild.SetParent(this, ref this.resetValue, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public ParentCollection Parents => new ParentCollection(this, 5).
            With(() => this.increment,  (ITriggerParent  parent) => this.increment  = parent).
            With(() => this.decrement,  (ITriggerParent  parent) => this.decrement  = parent).
            With(() => this.reset,      (ITriggerParent  parent) => this.reset      = parent).
            With(() => this.delta,      (IValueParent<T> parent) => this.delta      = parent).
            With(() => this.resetValue, (IValueParent<T> parent) => this.resetValue = parent);

        /// <summary>This sets the value of this node.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.UpdateValue(value);

        /// <summary>This is called when the value is evaluated and updated.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override T CalcuateValue() {
            T value = this.Value;
            T delta = this.delta is null ? default(T).OneValue : this.delta.Value;
            if (this.increment?.Provoked ?? false) value = value.Sum(new T[] { value, delta });
            if (this.decrement?.Provoked ?? false) value = value.Sub(delta);
            if (this.reset?.Provoked     ?? false) value = this.resetValue is null ? default : this.resetValue.Value;
            return value;
        }
    }
}
