using Blackboard.Core.Bases;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Determinines the minimum float value from all the parents.</summary>
    public class MinFloat: Nary<double, double> {

        /// <summary>Updates this node's value to the minimum value during evaluation.</summary>
        /// <param name="values">The parents' values to get the min of.</param>
        /// <returns>The minimum value from all the parents.</returns>
        protected override double OnEval(IEnumerable<double> values) {
            bool first = true;
            double result = default;
            foreach (double value in values) {
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
        public override string ToString() => "MinFloat"+base.ToString();
    }
}
