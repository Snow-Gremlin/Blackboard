using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This is a trigger which will be provoked when all of its non-null parents are provoked.</summary>
    sealed public class All: Multitrigger {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory = new FunctionN<ITrigger>((values) => new All(values));

        /// <summary>Creates an all trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public All(params ITrigger[] parents) :
            base(parents) { }

        /// <summary>Creates an all trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public All(IEnumerable<ITrigger> parents = null) :
            base(parents) { }

        /// <summary>Checks if all of the parents are provoked during evaluation.</summary>
        /// <param name="provoked">The provoked values from the parents.</param>
        /// <returns>True if all the parents are provoked, false otherwise.</returns>
        protected override bool OnEval(IEnumerable<bool> provoked) {
            foreach (bool trig in provoked) {
                if (!trig) return false;
            }
            return true;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "All"+base.ToString();
    }
}
