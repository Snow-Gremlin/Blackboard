using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a left shifts the first integer parent the amount of the second parent.</summary>
    public class LeftShift: Binary<int, int, int> {

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
