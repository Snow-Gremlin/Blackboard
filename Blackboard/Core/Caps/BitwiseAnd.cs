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

        public override string ToString() => "BitwiseAnd"+base.ToString();
    }
}
