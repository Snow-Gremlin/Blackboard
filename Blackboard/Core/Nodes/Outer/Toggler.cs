using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>This is a boolean value which can be toggled by triggers.</summary>
    sealed public class Toggler: ValueNode<Bool>, IChild {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        /// <remarks>This will not initialize a reset or resetValue sources which can be set later.</remarks>
        static public readonly IFuncDef Factory =
            new Function<ITriggerParent, Toggler>((ITriggerParent toggle) => new Toggler(toggle));

        /// <summary>This is the parent to toggle the value.</summary>
        private ITriggerParent toggle;

        /// <summary>This is the parent reset the toggle to reset parent's value.</summary>
        private ITriggerParent reset;

        /// <summary>This is the parent holding the value to reset with.</summary>
        private IValueParent<Bool> resetValue;

        /// <summary>Creates a toggling value node.</summary>
        /// <param name="toggle">The initial parent to toggle the value.</param>
        /// <param name="reset">The initial parent reset the toggle to reset parent's value.</param>
        /// <param name="resetValue">The initial parent for the value to reset to.</param>
        /// <param name="value">The initial boolean value for this node.</param>
        public Toggler(ITriggerParent toggle = null, ITriggerParent reset = null,
            IValueParent<Bool> resetValue = null, Bool value = default) : base(value) {
            this.Toggle = toggle;
            this.Reset = reset;
            this.ResetValue = resetValue;
        }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Toggler";

        /// <summary>This is the parent to toggle the value.</summary>
        public ITriggerParent Toggle {
            get => this.toggle;
            set => this.SetParent(ref this.toggle, value);
        }

        /// <summary>This is the parent reset the toggle to false.</summary>
        public ITriggerParent Reset {
            get => this.reset;
            set => this.SetParent(ref this.reset, value);
        }

        /// <summary>The value to reset this toggle to when the toggle is reset.</summary>
        /// <remarks>If this parent is null then the toggle is reset to false.</remarks>
        public IValueParent<Bool> ResetValue {
            get => this.resetValue;
            set => this.SetParent(ref this.resetValue, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => IChild.EnumerateParents(this.toggle, this.reset, this.resetValue);


        /// <summary>This will determine the new value the node should be set to.</summary>
        /// <returns>The new value that the node should be set to.</returns>
        protected override Bool CalcuateValue() {
            Bool value = this.Value;
            if (this.toggle?.Provoked ?? false) value = new(!value.Value);
            if (this.reset?.Provoked  ?? false) value = this.resetValue?.Value ?? Bool.False;
            return value;
        }
    }
}
