using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;
using System;

namespace Blackboard.Core.Caps {

    public class AbsInt: Unary<int, int> {

        public AbsInt(IValue<int> source = null, int value = default) :
            base(source, value) { }

        protected override int OnEval(int value) => Math.Abs(value);

        public override string ToString() => "AbsInt"+base.ToString();
    }
}
