using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>This will return the linear interpolation between two parent values.</summary>
    sealed public class Lerp: Ternary<double, double, double, double> {

        /// <summary>Creates a selection value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="source3">This is the third parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Lerp(IValue<double> source1 = null, IValue<double> source2 = null,
            IValue<double> source3 = null, double value = default) :
            base(source1, source2, source3, value) { }

        /// <summary>Selects the value to return during evaluation.</summary>
        /// <param name="value1">
        /// The value from the first parent. This is the iterator value.
        /// Zero or less will return the value from the second parent.
        /// One or more will return the value from the third parent.
        /// Between zero and one will return the linear interpolation.
        /// </param>
        /// <param name="value2">The first value to return.</param>
        /// <param name="value3">The second value to return.</param>
        /// <returns>The selected value to set to this node.</returns>
        protected override double OnEval(double value1, double value2, double value3) =>
            (value1 <= 0.0) ? value2 : (value1 >= 1.0) ? value3 : (value3-value2)*value1 + value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Lerp"+base.ToString();
    }
}
