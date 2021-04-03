using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean implies of two boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/Implies.html"/>
    public class Implies: Binary<bool, bool, bool> {

        /// <summary>Determines the boolean implies value of the two parents.</summary>
        /// <param name="value1">The first parent being implied</param>
        /// <param name="value2">The second parent implied.</param>
        /// <returns>The boolean implies value of the two given parents.</returns>
        protected override bool OnEval(bool value1, bool value2) => !value1 || value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Implies"+base.ToString();
    }
}
