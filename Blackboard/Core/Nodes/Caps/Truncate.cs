using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This gets the truncated value from the parent.</summary>
    sealed public class Truncate<T>: Unary<T, T>
        where T : IFloatingPoint<T>, IComparable<T>, new() {

        /// <summary>Creates a truncated value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Truncate(IValue<T> source = null, T value = default) :
            base(source, value) { }

        /// <summary>Truncates the parent's value during evaluation.</summary>
        /// <param name="value">The value to truncate.</param>
        /// <returns>The truncated value.</returns>
        protected override T OnEval(T value) => value.Truncate();

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Truncate"+base.ToString();
    }
}
