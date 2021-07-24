using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System;

namespace Blackboard.Core.Caps {

    /// <summary>This gets the rounded double value from the parent.</summary>
    sealed public class Round: Binary<double, int, double> {

        /// <summary>Creates a rounded value node.</summary>
        /// <param name="source1">This is the value parent for the source value.</param>
        /// <param name="source2">This is the decimals parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Round(IValue<double> source1 = null, IValue<int> source2 = null, double value = default) :
            base(source1, source2, value) { }

        /// <summary>Rounds the parent's value during evaluation.</summary>
        /// <param name="value1">The value to round.</param>
        /// <param name="value2">The number of decimals to round to.</param>
        /// <returns>The rounded value as an double.</returns>
        protected override double OnEval(double value1, int value2) => Math.Round(value1, value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Round"+base.ToString();
    }
}
