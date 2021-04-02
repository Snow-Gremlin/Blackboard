using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class LeftShift: Binary<int, int, int> {

        protected override int OnEval(int value1, int value2) => value1 << value2;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "LeftShift"+base.ToString();
    }
}
