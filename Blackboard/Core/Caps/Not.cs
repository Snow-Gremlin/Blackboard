using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean NOT of one boolean parent.</summary>
    /// <see cref="https://mathworld.wolfram.com/NOT.html"/>
    public class Not: Unary<bool, bool> {

        /// <summary>Gets the boolean NOT of the given parent during evaluation.</summary>
        /// <param name="value">The parent value to get the NOT of.</param>
        /// <returns>The NOT of the given parent value.</returns>
        protected override bool OnEval(bool value) => !value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Not"+base.ToString();
    }
}
