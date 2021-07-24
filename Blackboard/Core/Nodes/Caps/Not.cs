﻿using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a boolean NOT of one boolean parent.</summary>
    /// <see cref="https://mathworld.wolfram.com/NOT.html"/>
    sealed public class Not: Unary<bool, bool> {

        /// <summary>Creates a boolean NOT value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Not(IValue<bool> source = null, bool value = default) :
            base(source, value) { }

        /// <summary>Gets the boolean NOT of the given parent during evaluation.</summary>
        /// <param name="value">The parent value to get the NOT of.</param>
        /// <returns>The NOT of the given parent value.</returns>
        protected override bool OnEval(bool value) => !value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Not"+base.ToString();
    }
}