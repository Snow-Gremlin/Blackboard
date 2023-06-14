using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Provokes when one and only one of the parents is provoked.</summary>
    sealed public class OnlyOne: NaryTrigger {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory(inputs => new OnlyOne(inputs));

        /// <summary>Creates a one and only one trigger node.</summary>
        public OnlyOne() { }

        /// <summary>Creates a one and only one trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnlyOne(params ITriggerParent[] parents) : base(parents) { }

        /// <summary>Creates a one and only one trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnlyOne(IEnumerable<ITriggerParent> parents) : base(parents) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new OnlyOne();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(OnlyOne);

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
