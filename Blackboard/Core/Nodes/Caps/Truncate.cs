using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This gets the truncated double value from the parent as an integer.</summary>
    sealed public class Truncate: Unary<double, int> {

        /// <summary>Creates a truncated value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Truncate(IValue<double> source = null, int value = default) :
            base(source, value) { }

        /// <summary>Truncates the parent's value during evaluation.</summary>
        /// <param name="value">The value to truncate.</param>
        /// <returns>The truncated value as an integer.</returns>
        protected override int OnEval(double value) => (int)value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Truncate"+base.ToString();
    }
}
