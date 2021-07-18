using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using S = System;

namespace Blackboard.Core.Caps {

    /// <summary>This will return the value limitted to a range.</summary>
    sealed public class Clamp<T>: Ternary<T, T, T, T> where T: S.IComparable<T> {

        /// <summary>Creates a clamped value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="source3">This is the third parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Clamp(IValue<T> source1 = null, IValue<T> source2 = null,
            IValue<T> source3 = null, T value = default) :
            base(source1, source2, source3, value) { }

        /// <summary>Selects the value to return during evaluation.</summary>
        /// <param name="value1">
        /// The value from the first parent. This is the input value to clamp.
        /// If less than the second value then the second value is returned.
        /// If greater than the third value then the third value is returned.
        /// Otherwise the input value is returned when between the two values.
        /// </param>
        /// <param name="value2">The first value to return.</param>
        /// <param name="value3">The second value to return.</param>
        /// <returns>The clamped value to set to this node.</returns>
        protected override T OnEval(T value1, T value2, T value3) =>
            value1.CompareTo(value2) < 0 ? value2 :
            value1.CompareTo(value3) > 0 ? value3 :
            value1;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Clamp"+base.ToString();
    }
}
