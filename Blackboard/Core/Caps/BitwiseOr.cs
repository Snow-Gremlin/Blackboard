using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise OR of all the integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/OR.html"/>
    public class BitwiseOr: Nary<int, int> {

        /// <summary>Creates a bitwise OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseOr(params IValue<int>[] parents) :
            base(parents) { }

        /// <summary>Creates a bitwise OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public BitwiseOr(IEnumerable<IValue<int>> parents = null, int value = default) :
            base(parents, value) { }

        /// <summary>Gets the bitwise OR of all the parent's booleans.</summary>
        /// <param name="values">The to bitwise OR together.</param>
        /// <returns>The bitwise OR of all the given values.</returns>
        protected override int OnEval(IEnumerable<int> values) {
            int result = 0;
            foreach (int value in values) {
                result |= value;
            }
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "BitwiseOr"+base.ToString();
    }
}
