using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a power of two integer parents.</summary>
    sealed public class PowerInt: Binary<int, int, int> {

        /// <summary>Creates a power value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public PowerInt(IValue<int> source1 = null, IValue<int> source2 = null, int value = default) :
            base(source1, source2, value) { }

        /// <summary>Gets the power of the two parents.</summary>
        /// <param name="value1">The first parent's value in the power.</param>
        /// <param name="value2">The second parent's value in the power.</param>
        /// <returns>The XOR'ed value.</returns>
        protected override int OnEval(int value1, int value2) => (int)System.Math.Pow(value1,  value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Power"+base.ToString();
    }
}
