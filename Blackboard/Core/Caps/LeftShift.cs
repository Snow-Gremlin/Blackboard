using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class LeftShift: Binary<int, int, int> {

        protected override int OnEval(int value1, int value2) => value1 << value2;

        public override string ToString() => "LeftShift"+base.ToString();
    }
}
