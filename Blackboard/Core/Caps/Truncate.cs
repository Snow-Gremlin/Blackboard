using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;
using System;

namespace Blackboard.Core.Caps {

    public class Truncate: Unary<double, int> {

        protected override int OnEval(double value) => (int)value;

        public override string ToString() => "Truncate"+base.ToString();
    }
}
