using Blackboard.Core.Bases;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>This gets the product of all the float parents.</summary>
    public class MulFloat: Nary<double, double> {

        /// <summary>Gets the product of the parent values during evaluation.</summary>
        /// <param name="values">All the parent values to multiply.</param>
        /// <returns>The product of the parent values.</returns>
        protected override double OnEval(IEnumerable<double> values) {
            double result = 1.0;
            foreach (double value in values) result *= value;
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "MulFloat"+base.ToString();
    }
}
