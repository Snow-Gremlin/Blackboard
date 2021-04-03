using Blackboard.Core.Bases;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Determinines the minimum integer value from all the parents.</summary>
    public class MinInt: Nary<int, int> {

        /// <summary>Updates this node's value to the minimum value during evaluation.</summary>
        /// <param name="values">The parents' values to get the min of.</param>
        /// <returns>The minimum value from all the parents.</returns>
        protected override int OnEval(IEnumerable<int> values) {
            bool first = true;
            int result = default;
            foreach (int value in values) {
                if (first) {
                    result = value;
                    first = false;
                }
                if (value < result) result = value;
            }
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "MinInt"+base.ToString();
    }
}
