using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Determinines the maximum float value from all the parents.</summary>
    public class MaxFloat: Nary<double, double> {

        /// <summary>Creates a maximum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public MaxFloat(params IValue<double>[] parents) :
            base(parents) { }

        /// <summary>Creates a maximum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public MaxFloat(IEnumerable<IValue<double>> parents = null, double value = default) :
            base(parents, value) { }

        /// <summary>Updates this node's value to the maximum value during evaluation.</summary>
        /// <param name="values">The parents' values to get the max of.</param>
        /// <returns>The maximum value from all the parents.</returns>
        protected override double OnEval(IEnumerable<double> values) {
            bool first = true;
            double result = default;
            foreach (double value in values) {
                if (first) {
                    result = value;
                    first = false;
                }
                if (value > result) result = value;
            }
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "MaxFloat"+base.ToString();
    }
}
