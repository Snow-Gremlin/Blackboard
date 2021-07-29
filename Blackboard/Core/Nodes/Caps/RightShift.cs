using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a right shifts the first parent the amount of the second parent.</summary>
    sealed public class RightShift<T>: Binary<T, T, T>
        where T : IBitwise<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new Func<IValue<T>, IValue<T>>((left, right) => new RightShift<T>(left, right));

        /// <summary>Creates a right shift value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public RightShift(IValue<T> source1 = null, IValue<T> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>Right shifts the value during evaluation.</summary>
        /// <param name="value1">The integer value to right shift.</param>
        /// <param name="value2">The integer value to right shift the other value by.</param>
        /// <returns>The right shifted value for this node.</returns>
        protected override T OnEval(T value1, T value2) => value1.RightShift(value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "RightShift"+base.ToString();
    }
}
