using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class Div: Binary<double, double, double> {

        protected override double OnEval(double value1, double value2) =>
            value2 == 0.0 ? 0.0 : value1/value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Div"+base.ToString();
    }
}
