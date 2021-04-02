using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class NegFloat: Unary<double, double> {

        protected override double OnEval(double value) => -value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "NegFloat"+base.ToString();
    }
}
