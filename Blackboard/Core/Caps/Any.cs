using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class Any: Multitrigger {

        protected override bool OnEval(IEnumerable<bool> triggered) {
            foreach (bool trig in triggered) {
                if (trig) return true;
            }
            return false;
        }

        public override string ToString() => "Any"+base.ToString();
    }
}
