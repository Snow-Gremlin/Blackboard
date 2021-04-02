using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class MinFloat: Nary<double, double> {

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
