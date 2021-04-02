using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class All: Multitrigger {

        protected override bool OnEval(IEnumerable<bool> triggered) {
            foreach (bool trig in triggered) {
                if (!trig) return false;
            }
            return true;
        }

        public override string ToString() => "All"+base.ToString();
    }
}
