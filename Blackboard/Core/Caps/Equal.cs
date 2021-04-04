﻿using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Determines if the two values are equal.</summary>
    /// <typeparam name="T">The type being compared.</typeparam>
    public class Equal<T>: Binary<T, T, bool> {

        /// <summary>Creates an equal value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Equal(IValue<T> source1 = null, IValue<T> source2 = null, bool value = default) :
            base(source1, source2, value) { }

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
