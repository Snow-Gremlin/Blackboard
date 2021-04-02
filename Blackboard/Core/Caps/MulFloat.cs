using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class MulFloat: Nary<double, double> {

        protected override double OnEval(IEnumerable<double> values) {
            double result = 1.0;
            foreach (double value in values) result *= value;
            return result;
        }

        public override string ToString() => "MulFloat"+base.ToString();
    }
}
