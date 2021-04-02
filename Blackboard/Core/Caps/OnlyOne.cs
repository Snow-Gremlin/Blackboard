using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class OnlyOne: Multitrigger {

        protected override bool OnEval(IEnumerable<bool> triggered) {
            bool foundTriggered = false;
            foreach (bool trig in triggered) {
                if (trig) {
                    if (foundTriggered) return false;
                    foundTriggered = true;
                }
            }
            return foundTriggered;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "OnlyOne"+base.ToString();
    }
}
