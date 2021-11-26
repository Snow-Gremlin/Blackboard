using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Gets the average of all the inputs.</summary>
    sealed public class Average: NaryValue<Double, Double> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<IValueParent<Double>, Average>((values) => new Average(values));

        /// <summary>Creates an average value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Average(params IValueParent<Double>[] parents) :
            base(parents) { }

        /// <summary>Creates an average value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Average(IEnumerable<IValueParent<Double>> parents = null, Double value = default) :
            base(parents, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Average";

        /// <summary>Gets the average of all the parent's values.</summary>
        /// <param name="values">The parents to average together.</param>
        /// <returns>The average of all the given values.</returns>
        protected override Double OnEval(IEnumerable<Double> values) {
            int count = values.Count();
            if (count <= 0) return new(0.0);

            double sum = values.Sum(v => v.Value);
            return new(sum/count);
        }
    }
}
