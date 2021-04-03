using Blackboard.Core.Bases;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>This gets the product of all the integer parents.</summary>
    public class MulInt: Nary<int, int> {

        /// <summary>Gets the product of the parent values during evaluation.</summary>
        /// <param name="values">All the parent values to multiply.</param>
        /// <returns>The product of the parent values.</returns>
        protected override int OnEval(IEnumerable<int> values) {
            int result = 1;
            foreach (int value in values) result *= value;
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "MulInt"+base.ToString();
    }
}
