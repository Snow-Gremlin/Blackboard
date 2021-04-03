using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>This will return the value of one of two parents based on a boolean parent.</summary>
    /// <remarks>This functions just like a typical ternary statement.</remarks>
    /// <typeparam name="T">The type of input for the two value providing parents.</typeparam>
    public class Select<T>: Ternary<bool, T, T, T> {

        /// <summary>Selects the value to return during evaluation.</summary>
        /// <param name="value1">
        /// The parent to select with,
        /// true will return the second parent's value,
        /// false will return the third parent's value.
        /// </param>
        /// <param name="value2">The value to return if the first parent is true.</param>
        /// <param name="value3">The value to return if the second parent is true.</param>
        /// <returns>The selected value to set to this node.</returns>
        protected override T OnEval(bool value1, T value2, T value3) => value1 ? value2 : value3;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Select"+base.ToString();
    }
}
