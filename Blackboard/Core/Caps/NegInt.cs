using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>Negates the given parents value.</summary>
    sealed public class NegInt: Unary<int, int> {

        /// <summary>Creates a negated value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public NegInt(IValue<int> source = null, int value = default) :
            base(source, value) { }

        /// <summary>Gets the negated value of the parent during evaluation.</summary>
        /// <param name="value">The parent value to negate.</param>
        /// <returns>The negated parent value.</returns>
        protected override int OnEval(int value) => -value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "NegateInt"+base.ToString();
    }
}
