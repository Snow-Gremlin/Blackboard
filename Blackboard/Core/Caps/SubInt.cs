using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    public class SubInt: Binary<int, int, int> {

        public SubInt(IValue<int> source1 = null, IValue<int> source2 = null, int value = default) :
            base(source1, source2, value) { }

        protected override int OnEval(int value1, int value2) => value1-value2;

        public override string ToString() => "SubInt"+base.ToString();
    }
}
