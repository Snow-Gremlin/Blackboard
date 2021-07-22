using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Triggers a when one and only one of the parents is provoked.</summary>
    sealed public class OnlyOne: Multitrigger {

        /// <summary>Creates a one and only one trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnlyOne(params ITrigger[] parents) :
            base(parents) { }

        /// <summary>Creates a one and only one trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnlyOne(IEnumerable<ITrigger> parents = null) :
            base(parents) { }

        /// <summary>Updates this trigger during evaluation.</summary>
        /// <param name="provoked">The parent triggers to check.</param>
        /// <returns>True if one and only one parent was provoked.</returns>
        protected override bool OnEval(IEnumerable<bool> provoked) {
            bool foundProvoked = false;
            foreach (bool trig in provoked) {
                if (trig) {
                    if (foundProvoked) return false;
                    foundProvoked = true;
                }
            }
            return foundProvoked;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "OnlyOne"+base.ToString();
    }
}
