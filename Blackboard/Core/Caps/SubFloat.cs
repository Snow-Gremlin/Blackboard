using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class SubFloat: Binary<double, double, double> {

        protected override double OnEval(double value1, double value2) => value1-value2;

        public override string ToString() => "SubFloat"+base.ToString();
    }
}
