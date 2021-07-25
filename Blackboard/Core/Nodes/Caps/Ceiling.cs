using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This gets the ceiling value from the parent.</summary>
    sealed public class Ceiling<T>: Unary<T, T>
        where T : IFloatingPoint<T>, IComparable<T>, new() {

        /// <summary>Creates a ceiling value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Ceiling(IValue<T> source = null, T value = default) :
            base(source, value) { }

        /// <summary>Ceilings the parent's value during evaluation.</summary>
        /// <param name="value">The value to ceiling.</param>
        /// <returns>The ceiling value.</returns>
        protected override T OnEval(T value) => value.Ceiling();

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Ceiling"+base.ToString();
    }
}
