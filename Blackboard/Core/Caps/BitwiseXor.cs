using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise Exclusive OR of two integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    sealed public class BitwiseXor: Binary<int, int, int> {

        /// <summary>Creates a bitwise Exclusive OR value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public BitwiseXor(IValue<int> source1 = null, IValue<int> source2 = null, int value = default) :
            base(source1, source2, value) { }

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
