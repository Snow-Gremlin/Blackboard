using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class SubFloat: Binary<double, double, double> {

        public SubFloat(IValue<double> source1 = null, IValue<double> source2 = null, double value = default) :
            base(source1, source2, value) { }

        protected override double OnEval(double value1, double value2) => value1-value2;

        public override string ToString() => "SubFloat"+base.ToString();
    }
}
