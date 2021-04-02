using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean OR of all the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/OR.html"/>
    public class Or: Nary<bool, bool> {

        protected override bool OnEval(IEnumerable<bool> values) {
            foreach (bool value in values) {
                if (value) return true;
            }
            return false;
        }

        public override string ToString() => "Or"+base.ToString();
    }
}
