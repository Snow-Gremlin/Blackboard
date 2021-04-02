using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise Exclusive OR of two integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    public class BitwiseXor: Binary<int, int, int> {

        public BitwiseXor(IValue<int> source1 = null, IValue<int> source2 = null, int value = default) :
            base(source1, source2, value) { }

        protected override int OnEval(int value1, int value2) => value1 ^ value2;

        public override string ToString() => "BitwiseXor"+base.ToString();
    }
}
