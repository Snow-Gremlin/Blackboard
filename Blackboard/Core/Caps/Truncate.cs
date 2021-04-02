using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;
using System;

namespace Blackboard.Core.Caps {

    public class Truncate: Unary<double, int> {

        public Truncate(IValue<double> source = null, int value = default) :
            base(source, value) { }

        protected override int OnEval(double value) => (int)value;

        public override string ToString() => "Truncate"+base.ToString();
    }
}
