using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This is a trigger which will be provoked when any of its non-null parents are provoked.</summary>
    sealed public class Any: NaryTrigger {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((values) => new Any(values));

        /// <summary>Creates an any trigger node.</summary>
        public Any() { }

        /// <summary>Creates an any trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Any(params ITriggerParent[] parents) : base(parents) { }

        /// <summary>Creates an any trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Any(IEnumerable<ITriggerParent> parents = null) : base(parents) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Any();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Any);

        /// <summary>Checks if any of the parents are provoked during evaluation.</summary>
        /// <param name="provoked">The provoked values from the parents.</param>
        /// <returns>True if any of the parents are provoked, false otherwise.</returns>
        protected override bool OnEval(IEnumerable<bool> provoked) => provoked.Any(p => p);
    }
}
