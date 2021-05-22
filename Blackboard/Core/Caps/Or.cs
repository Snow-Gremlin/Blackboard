using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean OR of all the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/OR.html"/>
    sealed public class Or: Nary<bool, bool> {

        /// <summary>Creates a boolean OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Or(params IValue<bool>[] parents) :
            base(parents) { }

        /// <summary>Creates a boolean OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Or(IEnumerable<IValue<bool>> parents = null, bool value = default) :
            base(parents, value) { }

        /// <summary>Gets the OR of all the parent's booleans.</summary>
        /// <param name="values">The to OR together.</param>
        /// <returns>The OR of all the given values.</returns>
        protected override bool OnEval(IEnumerable<bool> values) {
            foreach (bool value in values) {
                if (value) return true;
            }
            return false;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Or"+base.ToString();
    }
}
