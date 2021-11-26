using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a boolean AND of all the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/AND.html"/>
    sealed public class And: NaryValue<Bool, Bool> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<IValueParent<Bool>, And>((values) => new And(values));

        /// <summary>Creates a boolean AND value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public And(params IValueParent<Bool>[] parents) :
            base(parents) { }

        /// <summary>Creates a boolean AND value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public And(IEnumerable<IValueParent<Bool>> parents = null, Bool value = default) :
            base(parents, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "And";

        /// <summary>Gets the AND of all the parent's booleans.</summary>
        /// <param name="values">The parents to AND together.</param>
        /// <returns>The AND of all the given values.</returns>
        protected override Bool OnEval(IEnumerable<Bool> values) => new(values.All(val => val.Value));
    }
}
