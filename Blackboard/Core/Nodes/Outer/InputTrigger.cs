using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>This is a trigger which can be provoked from user input.</summary>
    sealed public class InputTrigger: TriggerNode, ITriggerInput {

        /// <summary>Creates a new input trigger.</summary>
        public InputTrigger() { }

        /// <summary>Creates a new input trigger.</summary>
        /// <param name="provoked">The initial provoked state of the trigger.</param>
        public InputTrigger(bool provoked = false) :
            base(provoked) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new InputTrigger();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Input"; // So that it is Input<bool> and Input<trigger>.

        /// <summary>Provokes this trigger so that this node is provoked during the next evaluation.</summary>
        /// <remarks>This is not intended to be called directly, it should be called via the slate or action.</remarks>
        /// <param name="value">True will provoke, false will reset the trigger.</param>
        /// <returns>True if there was any change, false otherwise.</returns>
        public bool Provoke(bool value = true) => this.UpdateProvoked(value);

        /// <summary>This updates the trigger during the an evaluation.</summary>
        /// <returns>This returns the provoked value as it currently is.</returns>
        protected override bool ShouldProvoke() => this.Provoked;
    }
}
