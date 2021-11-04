using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a boolean OR of all the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/OR.html"/>
    sealed public class Or: Nary<Bool, Bool> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<IValueAdopter<Bool>, Or>((inputs) => new Or(inputs));

        /// <summary>Creates a boolean OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Or(params IValueAdopter<Bool>[] parents) :
            base(parents) { }

        /// <summary>Creates a boolean OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Or(IEnumerable<IValueAdopter<Bool>> parents = null, Bool value = default) :
            base(parents, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Or";

        /// <summary>Gets the OR of all the parent's booleans.</summary>
        /// <param name="values">The parents to OR together.</param>
        /// <returns>The OR of all the given values.</returns>
        protected override Bool OnEval(IEnumerable<Bool> values) {
            foreach (Bool value in values) {
                if (value.Value) return new(true);
            }
            return new(false);
        }
    }
}
