using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class NegInt: Unary<int, int> {

        public NegInt(IValue<int> source = null, int value = default) :
            base(source, value) { }

        protected override int OnEval(int value) => -value;

        public override string ToString() => "NegateInt"+base.ToString();
    }
}
