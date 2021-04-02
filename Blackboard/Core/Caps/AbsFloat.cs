using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;
using System;

namespace Blackboard.Core.Caps {

    public class AbsFloat: Unary<double, double> {

        public AbsFloat(IValue<double> source = null, double value = default) :
            base(source, value) { }

        protected override double OnEval(double value) => Math.Abs(value);

        public override string ToString() => "AbsFloat"+base.ToString();
    }
}
