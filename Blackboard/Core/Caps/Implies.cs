using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean implies of two boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/Implies.html"/>
    sealed public class Implies: Binary<bool, bool, bool> {

        /// <summary>Creates an implied value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Implies(IValue<bool> source1 = null, IValue<bool> source2 = null, bool value = default) :
            base(source1, source2, value) { }

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
