using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Negates the given parents value.</summary>
    public class NegInt: Unary<int, int> {

        /// <summary>Gets the negated value of the parent during evaluation.</summary>
        /// <param name="value">The parent value to negate.</param>
        /// <returns>The negated parent value.</returns>
        protected override int OnEval(int value) => -value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "NegateInt"+base.ToString();
    }
}
