using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>This will get the modulo the first parent value by the second parent value.</summary>
    sealed public class ModInt: Binary<int, int, int> {

        /// <summary>Creates a modulo value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public ModInt(IValue<int> source1 = null, IValue<int> source2 = null, int value = default) :
            base(source1, source2, value) { }

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
