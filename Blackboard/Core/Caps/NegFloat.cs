using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    public class NegFloat: Unary<double, double> {

        public NegFloat(IValue<double> source = null, double value = default) :
            base(source, value) { }

        protected override double OnEval(double value) => -value;

        public override string ToString() => "NegFloat"+base.ToString();
    }
}
