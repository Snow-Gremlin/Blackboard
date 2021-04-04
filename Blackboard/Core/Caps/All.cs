using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>This is a trigger which will be triggered when all of its non-null parents are triggered.</summary>
    public class All: Multitrigger {

        /// <summary>Creates an all trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public All(params ITrigger[] parents) :
            base(parents) { }

        /// <summary>Creates an all trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public All(IEnumerable<ITrigger> parents = null) :
            base(parents) { }

        /// <summary>Checks if all of the parents are triggered during evaluation.</summary>
        /// <param name="triggered">The triggered values from the parents.</param>
        /// <returns>True if all the parents are triggered, false otherwise.</returns>
        protected override bool OnEval(IEnumerable<bool> triggered) {
            foreach (bool trig in triggered) {
                if (!trig) return false;
            }
            return true;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "All"+base.ToString();
    }
}
