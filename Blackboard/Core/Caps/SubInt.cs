using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>Gets the difference between the two parent values.</summary>
    public class SubInt: Binary<int, int, int> {

        /// <summary>Creates a subtraction value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public SubInt(IValue<int> source1 = null, IValue<int> source2 = null, int value = default) :
            base(source1, source2, value) { }

        /// <summary>Gets the difference of the parents during evaluation.</summary>
        /// <param name="value1">The first value to be subtracted from.</param>
        /// <param name="value2">The second value to subtract from the first.</param>
        /// <returns>The difference of the two values.</returns>
        protected override int OnEval(int value1, int value2) => value1-value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "SubInt"+base.ToString();
    }
}
