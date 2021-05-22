using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a left shifts the first integer parent the amount of the second parent.</summary>
    sealed public class LeftShift: Binary<int, int, int> {

        /// <summary>Creates a left shift value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public LeftShift(IValue<int> source1 = null, IValue<int> source2 = null, int value = default) :
            base(source1, source2, value) { }

        /// <summary>Left shifts the value during evaluation.</summary>
        /// <param name="value1">The integer value to left shift.</param>
        /// <param name="value2">The integer value to left shift the other value by.</param>
        /// <returns>The left shifted value for this node.</returns>
        protected override int OnEval(int value1, int value2) => value1 << value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "LeftShift"+base.ToString();
    }
}
