using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This is a trigger which will be provoked when any of its non-null parents are provoked.</summary>
    sealed public class Any: Multitrigger {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<ITriggerAdopter, Any>((values) => new Any(values));

        /// <summary>Creates an any trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Any(params ITriggerAdopter[] parents) :
            base(parents) { }

        /// <summary>Creates an any trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Any(IEnumerable<ITriggerAdopter> parents = null) :
            base(parents) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Any";

        /// <summary>Checks if any of the parents are provoked during evaluation.</summary>
        /// <param name="provoked">The provoked values from the parents.</param>
        /// <returns>True if any of the parents are provoked, false otherwise.</returns>
        protected override bool OnEval(IEnumerable<bool> provoked) => provoked.Any(p => p);
    }
}
