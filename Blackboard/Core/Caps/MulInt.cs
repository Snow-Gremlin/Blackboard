using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class MulInt: Nary<int, int> {

        public MulInt(params IValue<int>[] sources) :
            this(sources as IEnumerable<IValue<int>>) { }

        public MulInt(IEnumerable<IValue<int>> sources = null, int value = default) :
            base(sources, value) { }

        protected override int OnEval(int[] values) {
            int result = 1;
            foreach (int value in values) result *= value;
            return result;
        }

        public override string ToString() => "MulInt"+base.ToString();
    }
}
