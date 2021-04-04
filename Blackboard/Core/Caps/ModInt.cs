using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>This will get the modulo the first parent value by the second parent value.</summary>
    public class ModInt: Binary<int, int, int> {

        /// <summary>Gets the first value moduled by a second value.</summary>
        /// <param name="value1">The first value to modulo.</param>
        /// <param name="value2">The second value to modulo.</param>
        /// <returns>The two values moduled, or the default if moduled by zero.</returns>
        protected override int OnEval(int value1, int value2) =>
            value2 == 0 ? default : value1%value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "ModFloat"+base.ToString();
    }
}
