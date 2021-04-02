using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class NegInt: Unary<int, int> {

        protected override int OnEval(int value) => -value;

        public override string ToString() => "NegateInt"+base.ToString();
    }
}
