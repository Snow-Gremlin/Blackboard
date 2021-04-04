using Blackboard.Core.Bases;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Determines if the two values are equal.</summary>
    /// <typeparam name="T">The type being compared.</typeparam>
    public class Equal<T>: Binary<T, T, bool> {

        /// <summary>Determine if the parent's values are equal during evaluation.</summary>
        /// <param name="value1">The first parent's value to compare.</param>
        /// <param name="value2">The second parent's value to compare.</param>
        /// <returns>True if the two values are equal, false otherwise.</returns>
        protected override bool OnEval(T value1, T value2) =>
            EqualityComparer<T>.Default.Equals(value1, value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Equal"+base.ToString();
    }
}
