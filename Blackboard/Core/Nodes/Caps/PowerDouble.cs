using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a power of two double parents.</summary>
    sealed public class PowerDouble: Binary<double, double, double> {

        /// <summary>Creates a power value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public PowerDouble(IValue<double> source1 = null, IValue<double> source2 = null, double value = default) :
            base(source1, source2, value) { }

        /// <summary>Gets the power of the two parents.</summary>
        /// <param name="value1">The first parent's value in the power.</param>
        /// <param name="value2">The second parent's value in the power.</param>
        /// <returns>The XOR'ed value.</returns>
        protected override double OnEval(double value1, double value2) => System.Math.Pow(value1, value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "PowerDouble"+base.ToString();
    }
}
