using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>This is a trigger which will be triggered when any of its non-null parents are triggered.</summary>
    public class Any: Multitrigger {

        /// <summary>Creates an any trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Any(params ITrigger[] parents) :
            base(parents) { }

        /// <summary>Creates an any trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Any(IEnumerable<ITrigger> parents = null) :
            base(parents) { }

        /// <summary>Checks if any of the parents are triggered during evaluation.</summary>
        /// <param name="triggered">The triggered values from the parents.</param>
        /// <returns>True if any of the parents are triggered, false otherwise.</returns>
        protected override bool OnEval(IEnumerable<bool> triggered) {
            foreach (bool trig in triggered) {
                if (trig) return true;
            }
            return false;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Any"+base.ToString();
    }
}
