using Blackboard.Core.Data.Caps;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Gets the average of all the inputs.</summary>
    sealed public class Average: Nary<Double, Double> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new FunctionN<IValue<Double>, Average>((values) => new Average(values));

        /// <summary>Creates an average value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Average(params IValue<Double>[] parents) :
            base(parents) { }

        /// <summary>Creates an average value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Average(IEnumerable<IValue<Double>> parents = null, Double value = default) :
            base(parents, value) { }

        /// <summary>Gets the average of all the parent's values.</summary>
        /// <param name="values">The parents to average together.</param>
        /// <returns>The average of all the given values.</returns>
        protected override Double OnEval(IEnumerable<Double> values) {
            int count = 0;
            double sum = 0.0;
            foreach (Double value in values) {
                sum += value.Value;
                count++;
            }
            return new(count <= 0 ? 0.0 : sum/count);
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Average"+base.ToString();
    }
}
