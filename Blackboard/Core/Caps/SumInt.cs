using Blackboard.Core.Bases;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Gets the sum of all of the parent values.</summary>
    public class SumInt: Nary<int, int> {

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
