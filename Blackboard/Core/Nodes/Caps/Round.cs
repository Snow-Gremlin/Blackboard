using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This gets the rounded value from the parent.</summary>
    sealed public class Round<T>: Binary<T, Int, T>
        where T : IFloatingPoint<T>, IComparable<T>, new() {

        /// <summary>Creates a rounded value node.</summary>
        /// <param name="source1">This is the value parent for the source value.</param>
        /// <param name="source2">This is the decimals parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Round(IValue<T> source1 = null, IValue<Int> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>Rounds the parent's value during evaluation.</summary>
        /// <param name="value1">The value to round.</param>
        /// <param name="value2">The number of decimals to round to.</param>
        /// <returns>The rounded value.</returns>
        protected override T OnEval(T value1, Int value2) => value1.Round(value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Round"+base.ToString();
    }
}
