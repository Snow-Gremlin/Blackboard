using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean AND of all the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/AND.html"/>
    public class And: Nary<bool, bool> {

        public And(params IValue<bool>[] sources) :
            this(sources as IEnumerable<IValue<bool>>) { }

        public And(IEnumerable<IValue<bool>> sources = null, bool value = default) :
            base(sources, value) { }

        protected override bool OnEval(bool[] values) {
            foreach (bool value in values) {
                if (!value) return false;
            }
            return true;
        }

        public override string ToString() => "And"+base.ToString();
    }
}
