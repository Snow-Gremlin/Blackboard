﻿using Blackboard.Core.Bases;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Determines if the two values are greater than.</summary>
    /// <typeparam name="T">The type being compared.</typeparam>
    public class GreaterThan<T>: Binary<T, T, bool> {

        /// <summary>Determine if the parent's values are greater than during evaluation.</summary>
        /// <param name="value1">The first parent's value to compare.</param>
        /// <param name="value2">The second parent's value to compare.</param>
        /// <returns>True if the first value is greater than than the second value, false otherwise.</returns>
        protected override bool OnEval(T value1, T value2) =>
            Comparer<T>.Default.Compare(value1, value2) > 0;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "GreaterThan"+base.ToString();
    }
}
