using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>This is a boolean value which can be toggled by triggers.</summary>
    public class Toggler: ValueNode<bool> {

        /// <summary>This is the parent to toggle the value.</summary>
        private ITrigger toggle;

        /// <summary>This is the parent reset the toggle to reset parent's value.</summary>
        private ITrigger reset;

        /// <summary>This is the parent holding the value to reset with.</summary>
        private ValueNode<bool> resetValue;

        /// <summary>Creates a toggling value node.</summary>
        /// <param name="toggle">The initial parent to toggle the value.</param>
        /// <param name="reset">The initial parent reset the toggle to reset parent's value.</param>
        /// <param name="resetValue">The initial parent for the value to reset to.</param>
        /// <param name="value">The initial boolean value for this node.</param>
        public Toggler(ITrigger toggle = null, ITrigger reset = null, ValueNode<bool> resetValue = null, bool value = false) : base(value) {
            this.Toggle = toggle;
            this.Reset = reset;
            this.ResetValue = resetValue;
        }

        /// <summary>This is the parent to toggle the value.</summary>
        public ITrigger Toggle {
            get => this.toggle;
            set => this.toggle = this.SetParent(this.toggle, value);
        }

        /// <summary>This is the parent reset the toggle to false.</summary>
        public ITrigger Reset {
            get => this.reset;
            set => this.reset = this.SetParent(this.reset, value);
        }

        /// <summary>The value to reset this toggle to when the toggle is reset.</summary>
        /// <remarks>If this parent is null then the toggle is reset to false.</remarks>
        public ValueNode<bool> ResetValue {
            get => this.resetValue;
            set => this.resetValue = this.SetParent(this.resetValue, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (!(this.toggle is null)) yield return this.toggle;
                if (!(this.reset is null)) yield return this.reset;
            }
        }

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override bool UpdateValue() {
            bool value = this.Value;
            if (this.toggle?.Triggered ?? false) value = !value;
            if (this.reset?.Triggered ?? false) value = this.resetValue?.Value ?? false;
            return this.SetNodeValue(value);
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Toggler("+NodeString(this.toggle)+", "+
            NodeString(this.reset)+", "+NodeString(this.resetValue)+")";
    }
}
