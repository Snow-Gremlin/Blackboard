using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Gets the sum of all of the parent values.</summary>
    public class SumFloat: Nary<double, double> {

        /// <summary>Creates a sum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public SumFloat(params IValue<double>[] parents) :
            base(parents) { }

        /// <summary>Creates a sum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public SumFloat(IEnumerable<IValue<double>> parents = null, double value = default) :
            base(parents, value) { }

        /// <summary>Gets the sum of all the parent values.</summary>
        /// <param name="values">The values to sum together.</param>
        /// <returns>The sum of the parent values.</returns>
        protected override double OnEval(IEnumerable<double> values) {
            double result = 0.0;
            foreach (double value in values) result += value;
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "SumFloat"+base.ToString();
    }
}
