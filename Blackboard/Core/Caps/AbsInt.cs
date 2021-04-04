using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System;

namespace Blackboard.Core.Caps {

    /// <summary>A integer value node that gets the absolute value of the parent.</summary>
    public class AbsInt: Unary<int, int> {

        /// <summary>Creates an absolute value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public AbsInt(IValue<int> source = null, int value = default) :
            base(source, value) { }

        /// <summary>This will get the absolute value of the parent's value on evaluation.</summary>
        /// <param name="value">The value to get the absolute of.</param>
        /// <returns>The absolute of the given value.</returns>
        protected override int OnEval(int value) => Math.Abs(value);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "AbsInt"+base.ToString();
    }
}
