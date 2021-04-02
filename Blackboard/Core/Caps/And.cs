using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean AND of all the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/AND.html"/>
    public class And: Nary<bool, bool> {

        protected override bool OnEval(IEnumerable<bool> values) {
            foreach (bool value in values) {
                if (!value) return false;
            }
            return true;
        }

        public override string ToString() => "And"+base.ToString();
    }
}
