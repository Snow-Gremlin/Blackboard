using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This will get the remainder the first parent value by the second parent value.</summary>
    sealed public class RemInt: Binary<int, int, int> {

        /// <summary>Creates a remainder value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public RemInt(IValue<int> source1 = null, IValue<int> source2 = null, int value = default) :
            base(source1, source2, value) { }

        /// <summary>Gets the first value remainder from a second value.</summary>
        /// <param name="value1">The first value to modulo.</param>
        /// <param name="value2">The second value to modulo.</param>
        /// <returns>The remainder, or the default if moduled by zero.</returns>
        protected override int OnEval(int value1, int value2) =>
            value2 == 0 ? default : value1 - (value1%value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "RemInt"+base.ToString();
    }
}
