﻿using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Triggers a when one and only one of the parents is provoked.</summary>
    sealed public class OnlyOne: Multitrigger {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<ITriggerAdopter, OnlyOne>((inputs) => new OnlyOne(inputs));

        /// <summary>Creates a one and only one trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnlyOne(params ITriggerAdopter[] parents) :
            base(parents) { }

        /// <summary>Creates a one and only one trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnlyOne(IEnumerable<ITriggerAdopter> parents = null) :
            base(parents) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "OnlyOne";

        /// <summary>Updates this trigger during evaluation.</summary>
        /// <param name="provoked">The parent triggers to check.</param>
        /// <returns>True if one and only one parent was provoked.</returns>
        protected override bool OnEval(IEnumerable<bool> provoked) {
            bool foundProvoked = false;
            foreach (bool trig in provoked) {
                if (trig) {
                    if (foundProvoked) return false;
                    foundProvoked = true;
                }
            }
            return foundProvoked;
        }
    }
}
