using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise OR of all the integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/OR.html"/>
    public class BitwiseOr: Nary<int, int> {

        protected override int OnEval(IEnumerable<int> values) {
            int result = 0;
            foreach (int value in values) {
                result |= value;
            }
            return result;
        }

        public override string ToString() => "BitwiseOr"+base.ToString();
    }
}
