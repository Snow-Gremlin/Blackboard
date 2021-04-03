using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>This gets the truncated float value from the parent as an integer.</summary>
    public class Truncate: Unary<double, int> {

        /// <summary>Truncates the parent's value during evaluation.</summary>
        /// <param name="value">The value to truncate.</param>
        /// <returns>The truncated value as an integer.</returns>
        protected override int OnEval(double value) => (int)value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Truncate"+base.ToString();
    }
}
