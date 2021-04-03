using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a right shifts the first integer parent the amount of the second parent.</summary>
    public class RightShift: Binary<int, int, int> {

        /// <summary>Right shifts the value during evaluation.</summary>
        /// <param name="value1">The integer value to right shift.</param>
        /// <param name="value2">The integer value to right shift the other value by.</param>
        /// <returns>The right shifted value for this node.</returns>
        protected override int OnEval(int value1, int value2) => value1 >> value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "RightShift"+base.ToString();
    }
}
