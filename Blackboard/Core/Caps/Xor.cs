using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean Exclusive OR of two boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    public class Xor: Binary<bool, bool, bool> {

        public Xor(IValue<bool> source1 = null, IValue<bool> source2 = null, bool value = default) :
            base(source1, source2, value) { }

        protected override bool OnEval(bool value1, bool value2) => value1 ^ value2;

        public override string ToString() => "Xor"+base.ToString();
    }
}
