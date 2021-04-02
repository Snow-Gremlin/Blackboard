using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise NOT of one integer parent.</summary>
    public class BitwiseNot: Unary<int, int> {

        protected override int OnEval(int value) => ~value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "BitwiseNot"+base.ToString();
    }
}
