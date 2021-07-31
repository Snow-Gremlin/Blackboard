using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Functions;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a power of two parents.</summary>
    sealed public class Power<T>: Binary<T, T, T>
        where T : IArithmetic<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new Function<IValue<T>, IValue<T>>((left, right) => new Power<T>(left, right));

        /// <summary>Creates a power value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Power(IValue<T> source1 = null, IValue<T> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>Gets the power of the two parents.</summary>
        /// <param name="value1">The first parent's value in the power.</param>
        /// <param name="value2">The second parent's value in the power.</param>
        /// <returns>The XOR'ed value.</returns>
        protected override T OnEval(T value1, T value2) => value1.Pow(value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Power"+base.ToString();
    }
}
