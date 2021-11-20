using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This is a trigger which will be provoked when all of its non-null parents are provoked.</summary>
    sealed public class All: Multitrigger {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<ITriggerAdopter, All>((values) => new All(values));

        /// <summary>Creates an all trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public All(params ITriggerAdopter[] parents) :
            base(parents) { }

        /// <summary>Creates an all trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public All(IEnumerable<ITriggerAdopter> parents = null) :
            base(parents) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "All";

        /// <summary>Checks if all of the parents are provoked during evaluation.</summary>
        /// <param name="provoked">The provoked values from the parents.</param>
        /// <returns>True if all the parents are provoked, false otherwise.</returns>
        protected override bool OnEval(IEnumerable<bool> provoked) => provoked.All(p => p);
    }
}
