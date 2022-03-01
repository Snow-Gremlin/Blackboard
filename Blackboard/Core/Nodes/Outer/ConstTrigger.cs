using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>
    /// This node should only be used by the parser while reducing nodes.
    /// This will store a trigger's provide state as a constant to take the
    /// place of the trigger this was made a constant of.
    /// </summary>
    sealed public class ConstTrigger: TriggerNode, IConstant {

        /// <summary>Creates a new constant trigger value node.</summary>
        public ConstTrigger() { }

        /// <summary>Creates a new constant trigger value node.</summary>
        /// <param name="provoked">The provoke state for this trigger.</param>
        public ConstTrigger(bool provoked = false) => this.SetProvoked(provoked);

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new ConstTrigger();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(ConstTrigger);

        /// <summary>This sets the value of the provoked constant.</summary>
        /// <param name="provoked">The state to set.</param>
        /// <returns>True if the state has changed, false otherwise.</returns>
        public bool SetProvoked(bool provoked) => this.UpdateProvoked(provoked);

        /// <summary>This will update the trigger during evaluation.</summary>
        /// <returns>True to trigger if the source value is true, false otherwise.</returns>
        protected override bool ShouldProvoke() => this.Provoked;
    }
}
