using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This is a trigger which will be provoked when any of its non-null parents are provoked.</summary>
    sealed public class Any: Multitrigger {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory = new FuncN<ITrigger>((values) => new Any(values));

        /// <summary>Creates an any trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Any(params ITrigger[] parents) :
            base(parents) { }

        /// <summary>Creates an any trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Any(IEnumerable<ITrigger> parents = null) :
            base(parents) { }

        /// <summary>Checks if any of the parents are provoked during evaluation.</summary>
        /// <param name="provoked">The provoked values from the parents.</param>
        /// <returns>True if any of the parents are provoked, false otherwise.</returns>
        protected override bool OnEval(IEnumerable<bool> provoked) {
            foreach (bool trig in provoked) {
                if (trig) return true;
            }
            return false;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Any"+base.ToString();
    }
}
