﻿using Blackboard.Core.Bases;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean AND of all the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/AND.html"/>
    public class And: Nary<bool, bool> {

        /// <summary>Gets the AND of all the parent's booleans.</summary>
        /// <param name="values">The to AND together.</param>
        /// <returns>The AND of all the given values.</returns>
        protected override bool OnEval(IEnumerable<bool> values) {
            foreach (bool value in values) {
                if (!value) return false;
            }
            return true;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "And"+base.ToString();
    }
}
