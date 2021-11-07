using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>This is a boolean value which can be toggled by triggers.</summary>
    sealed public class Toggler: ValueNode<Bool> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        /// <remarks>This will not initialize a reset or resetValue sources which can be set later.</remarks>
        static public readonly IFuncDef Factory =
            new Function<ITriggerAdopter, Toggler>((ITriggerAdopter toggle) => new Toggler(toggle));

        /// <summary>This is the parent to toggle the value.</summary>
        private ITriggerAdopter toggle;

        /// <summary>This is the parent reset the toggle to reset parent's value.</summary>
        private ITriggerAdopter reset;

        /// <summary>This is the parent holding the value to reset with.</summary>
        private IValueAdopter<Bool> resetValue;

        /// <summary>Creates a toggling value node.</summary>
        /// <param name="toggle">The initial parent to toggle the value.</param>
        /// <param name="reset">The initial parent reset the toggle to reset parent's value.</param>
        /// <param name="resetValue">The initial parent for the value to reset to.</param>
        /// <param name="value">The initial boolean value for this node.</param>
        public Toggler(ITriggerAdopter toggle = null, ITriggerAdopter reset = null,
            IValueAdopter<Bool> resetValue = null, Bool value = default) : base(value) {
            this.Toggle = toggle;
            this.Reset = reset;
            this.ResetValue = resetValue;
        }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Toggler";

        /// <summary>This is the parent to toggle the value.</summary>
        public ITriggerAdopter Toggle {
            get => this.toggle;
            set => this.SetParent(ref this.toggle, value);
        }

        /// <summary>This is the parent reset the toggle to false.</summary>
        public ITriggerAdopter Reset {
            get => this.reset;
            set => this.SetParent(ref this.reset, value);
        }

        /// <summary>The value to reset this toggle to when the toggle is reset.</summary>
        /// <remarks>If this parent is null then the toggle is reset to false.</remarks>
        public IValueAdopter<Bool> ResetValue {
            get => this.resetValue;
            set => this.SetParent(ref this.resetValue, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.toggle is not null) yield return this.toggle;
                if (this.reset is not null) yield return this.reset;
            }
        }

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override bool UpdateValue() {
            Bool value = this.Value;
            if (this.toggle?.Provoked ?? false) value = new(!value.Value);
            if (this.reset?.Provoked ?? false) value = this.resetValue?.Value ?? new(false);
            return this.SetNodeValue(value);
        }
    }
}
