using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class SumFloat: Nary<double, double> {

        public SumFloat(params IValue<double>[] sources) :
            this(sources as IEnumerable<IValue<double>>) { }

        public SumFloat(IEnumerable<IValue<double>> sources = null, int value = default) :
            base(sources, value) { }

        protected override double OnEval(double[] values) {
            double result = 0.0;
            foreach (double value in values) result += value;
            return result;
        }

        public override string ToString() => "SumFloat"+base.ToString();
    }
}
