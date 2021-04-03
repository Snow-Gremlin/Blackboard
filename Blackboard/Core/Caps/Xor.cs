using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean Exclusive OR of two boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    public class Xor: Binary<bool, bool, bool> {

        /// <summary>Gets the XOR of the two parents.</summary>
        /// <param name="value1">The first parent's value in the XOR.</param>
        /// <param name="value2">The second parent's value in the XOR.</param>
        /// <returns>The XOR'ed value.</returns>
        protected override bool OnEval(bool value1, bool value2) => value1 ^ value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Xor"+base.ToString();
    }
}
