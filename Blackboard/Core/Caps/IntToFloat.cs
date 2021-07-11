using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>Casts an integer node into a float node.</summary>
    sealed public class IntToFloat: Unary<int, double> {

        /// <summary>Creates a int to float value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public IntToFloat(IValue<int> source = null, int value = default) :
            base(source, value) { }

        /// <summary>Gets the float value of the parent during evaluation.</summary>
        /// <param name="value">The parent value to cast to float.</param>
        /// <returns>The float of the parent value.</returns>
        protected override double OnEval(int value) => value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "IntToFloat"+base.ToString();
    }
}
