using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise NOT of one integer parent.</summary>
    public class BitwiseNot: Unary<int, int> {

        /// <summary>Creates a bitwise NOT value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public BitwiseNot(IValue<int> source = null, int value = default) :
            base(source, value) { }

        /// <summary>Gets the bitwise NOT of the given value.</summary>
        /// <param name="value">The value to get the bitwise NOT of.</param>
        /// <returns>The bitwise NOT of the given value.</returns>
        protected override int OnEval(int value) => ~value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "BitwiseNot"+base.ToString();
    }
}
