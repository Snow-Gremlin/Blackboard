using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class Select<T>: Ternary<bool, T, T, T> {

        public Select(IValue<bool> source1 = null, IValue<T> source2 = null, IValue<T> source3 = null, T value = default) :
            base(source1, source2, source3, value) { }

        protected override T OnEval(bool value1, T value2, T value3) => value1 ? value2 : value3;

        public override string ToString() => "Select"+base.ToString();
    }
}
