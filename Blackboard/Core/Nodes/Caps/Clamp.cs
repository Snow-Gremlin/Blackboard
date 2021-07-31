using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This will return the value limitted to a range.</summary>
    sealed public class Clamp<T>: Ternary<T, T, T, T>
        where T : IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new Function<IValue<T>, IValue<T>, IValue<T>>((value1, value2, value3) => new Clamp<T>(value1, value2, value3));

        /// <summary>Creates a clamped value node.</summary>
        /// <param name="source1">This is the value parent that is clamped.</param>
        /// <param name="source2">This is the minimum value parent for the lower edge of the clamp.</param>
        /// <param name="source3">This is the maximum value parent for the upper edge of the clamp.</param>
        /// <param name="value">The default value for this node.</param>
        public Clamp(IValue<T> source1 = null, IValue<T> source2 = null,
            IValue<T> source3 = null, T value = default) :
            base(source1, source2, source3, value) { }

        /// <summary>Selects the value to return during evaluation.</summary>
        /// <param name="value">
        /// The value from the first parent. This is the input value to clamp.
        /// If less than the min value then the min value is returned.
        /// If greater than the max value then the max value is returned.
        /// Otherwise the input value is returned when between the other two values.
        /// </param>
        /// <param name="min">The minimum value to return.</param>
        /// <param name="max">The maximum value to return.</param>
        /// <returns>The clamped value to set to this node.</returns>
        protected override T OnEval(T value, T min, T max) =>
            value.CompareTo(min) < 0 ? min :
            value.CompareTo(max) > 0 ? max :
            value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Clamp"+base.ToString();
    }
}
