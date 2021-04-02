using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise Exclusive OR of two integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    public class BitwiseXor: Binary<int, int, int> {

        protected override int OnEval(int value1, int value2) => value1 ^ value2;

        public override string ToString() => "BitwiseXor"+base.ToString();
    }
}
