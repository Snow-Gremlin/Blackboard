﻿using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Determines if the two values are less than or equal.</summary>
    /// <typeparam name="T">The type being compared.</typeparam>
    sealed public class LessThanOrEqual<T>: Binary<T, T, Bool>
        where T : IComparable<T>, new() {

        /// <summary>Creates a less than or equal value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public LessThanOrEqual(IValue<T> source1 = null, IValue<T> source2 = null, Bool value = default) :
            base(source1, source2, value) { }

        /// <summary>Determine if the parent's values are less than or equal during evaluation.</summary>
        /// <param name="value1">The first parent's value to compare.</param>
        /// <param name="value2">The second parent's value to compare.</param>
        /// <returns>True if the first value is less than or equal than the second value, false otherwise.</returns>
        protected override Bool OnEval(T value1, T value2) => Bool.Wrap(value1.CompareTo(value2) <= 0);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "LessThanOrEqual"+base.ToString();
    }
}