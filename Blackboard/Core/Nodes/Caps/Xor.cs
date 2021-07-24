using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a boolean Exclusive OR of two boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    sealed public class Xor: Binary<bool, bool, bool> {

        /// <summary>Creates a boolean XOR value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Xor(IValue<bool> source1 = null, IValue<bool> source2 = null, bool value = default) :
            base(source1, source2, value) { }

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
