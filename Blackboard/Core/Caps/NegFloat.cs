using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class NegFloat: Unary<double, double> {

        protected override double OnEval(double value) => -value;

        public override string ToString() => "NegFloat"+base.ToString();
    }
}
