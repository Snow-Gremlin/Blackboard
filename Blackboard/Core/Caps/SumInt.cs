using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Gets the sum of all of the parent values.</summary>
    sealed public class SumInt: Nary<int, int> {

        /// <summary>Creates a sum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public SumInt(params IValue<int>[] parents) :
            base(parents) { }

        /// <summary>Creates a sum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public SumInt(IEnumerable<IValue<int>> parents = null, int value = default) :
            base(parents, value) { }

        /// <summary>Gets the sum of all the parent values.</summary>
        /// <param name="values">The values to sum together.</param>
        /// <returns>The sum of the parent values.</returns>
        protected override int OnEval(IEnumerable<int> values) {
            int result = 0;
            foreach (int value in values) result += value;
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "SumInt"+base.ToString();
    }
}
