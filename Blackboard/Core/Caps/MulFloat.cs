using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class MulFloat: Nary<double, double> {

        public MulFloat(params IValue<double>[] sources) :
            this(sources as IEnumerable<IValue<double>>) { }

        public MulFloat(IEnumerable<IValue<double>> sources = null, double value = default) :
            base(sources, value) { }

        protected override double OnEval(double[] values) {
            double result = 1.0;
            foreach (double value in values) result *= value;
            return result;
        }

        public override string ToString() => "MulFloat"+base.ToString();
    }
}
