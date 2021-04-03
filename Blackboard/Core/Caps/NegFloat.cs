using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Negates the given parents value.</summary>
    public class NegFloat: Unary<double, double> {

        /// <summary>Gets the negated value of the parent during evaluation.</summary>
        /// <param name="value">The parent value to negate.</param>
        /// <returns>The negated parent value.</returns>
        protected override double OnEval(double value) => -value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "NegFloat"+base.ToString();
    }
}
