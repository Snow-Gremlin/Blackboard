using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise Exclusive OR of two integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    public class BitwiseXor: Binary<int, int, int> {

        /// <summary>This will XOR of the two given values.</summary>
        /// <param name="value1">The first of the two values in the XOR.</param>
        /// <param name="value2">The second of the two values in the XOR.</param>
        /// <returns>The XOR of the two given values.</returns>
        protected override int OnEval(int value1, int value2) => value1 ^ value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "BitwiseXor"+base.ToString();
    }
}
