using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean OR of all the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/OR.html"/>
    public class Or: Nary<bool, bool> {

        public Or(params IValue<bool>[] sources) :
            this(sources as IEnumerable<IValue<bool>>) { }

        public Or(IEnumerable<IValue<bool>> sources = null, bool value = default) :
            base(sources, value) { }

        protected override bool OnEval(bool[] values) {
            foreach (bool value in values) {
                if (value) return true;
            }
            return false;
        }

        public override string ToString() => "Or"+base.ToString();
    }
}
