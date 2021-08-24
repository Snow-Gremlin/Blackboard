using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This is a trigger which can be provoked from user input.</summary>
    sealed public class InputTrigger: TriggerNode, ITriggerInput {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new Function<InputTrigger>(() => new InputTrigger());

        /// <summary>Creates a new input trigger.</summary>
        /// <param name="provoked">The initial provoked state of the trigger.</param>
        public InputTrigger(bool provoked = false) :
            base(provoked) { }

        /// <summary>Always returns no parents since inputs have no parent.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>This is set this trigger to emit during the next evaluation.</summary>
        /// <param name="value">True will provoke, false will reset the trigger.</param>
        /// <remarks>This is not intended to be be called directly, it should be called via the driver.</remarks>
        public void Provoke(bool value = true) => this.Provoked = value;

        /// <summary>This updates the trigger during the an evaluation.</summary>
        /// <returns>This returns the provoked value as it currently is.</returns>
        protected override bool UpdateTrigger() => this.Provoked;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Input<trigger>";
    }
}
