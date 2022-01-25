using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>
    /// 
    /// </summary>
    sealed internal class Coalescer : IRule {

        /// <summary>Reduce the parents and coalesce nodes as mush as possible.</summary>
        /// <param name="args">The arguments for the optimization rules.</param>
        public void Perform(RuleArgs args) {

            // DON'T forget to add ICoalescable to the trigger nodes.

            throw new S.NotImplementedException();
        }
    }
}
