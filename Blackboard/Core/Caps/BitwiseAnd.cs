using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise AND of all the integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/AND.html"/>
    public class BitwiseAnd: Nary<int, int> {

        protected override int OnEval(IEnumerable<int> values) {
            int result = int.MaxValue;
            foreach (int value in values) {
                result &= value;
            }
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "BitwiseAnd"+base.ToString();
    }
}
