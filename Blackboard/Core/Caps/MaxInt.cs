using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class MaxInt: Nary<int, int> {

        protected override int OnEval(IEnumerable<int> values) {
            bool first = true;
            int result = default;
            foreach (int value in values) {
                if (first) {
                    result = value;
                    first = false;
                }
                if (value > result) result = value;
            }
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "MaxInt"+base.ToString();
    }
}
