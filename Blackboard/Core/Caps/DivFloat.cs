using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>This will divide the first parent value by the second parent value.</summary>
    public class DivFloat: Binary<double, double, double> {

        /// <summary>Creates a divided value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public DivFloat(IValue<double> source1 = null, IValue<double> source2 = null, double value = default) :
            base(source1, source2, value) { }

        /// <summary>Gets the first value divided by a second value.</summary>
        /// <param name="value1">The first value to divide.</param>
        /// <param name="value2">The second value to divide.</param>
        /// <returns>The two values divided, or the default if divide by zero.</returns>
        protected override double OnEval(double value1, double value2) =>
            value2 == 0.0 ? default : value1/value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "DivFloat"+base.ToString();
    }
}
