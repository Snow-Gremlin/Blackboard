using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class MulInt: Nary<int, int> {

        protected override int OnEval(IEnumerable<int> values) {
            int result = 1;
            foreach (int value in values) result *= value;
            return result;
        }

        public override string ToString() => "MulInt"+base.ToString();
    }
}
