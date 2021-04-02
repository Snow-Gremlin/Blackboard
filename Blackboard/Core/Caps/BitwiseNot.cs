using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a bitwise NOT of one integer parent.</summary>
    public class BitwiseNot: Unary<int, int> {

        public BitwiseNot(IValue<int> source = null, int value = default) :
            base(source, value) { }

        protected override int OnEval(int value) => ~value;

        public override string ToString() => "BitwiseNot"+base.ToString();
    }
}
