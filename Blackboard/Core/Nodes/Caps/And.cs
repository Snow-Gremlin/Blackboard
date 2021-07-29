using Blackboard.Core.Data.Caps;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a boolean AND of all the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/AND.html"/>
    sealed public class And: Nary<Bool, Bool> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory = new FuncN<IValue<Bool>>((values) => new And(values));

        /// <summary>Creates a boolean AND value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public And(params IValue<Bool>[] parents) :
            base(parents) { }

        /// <summary>Creates a boolean AND value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public And(IEnumerable<IValue<Bool>> parents = null, Bool value = default) :
            base(parents, value) { }

        /// <summary>Gets the AND of all the parent's booleans.</summary>
        /// <param name="values">The to AND together.</param>
        /// <returns>The AND of all the given values.</returns>
        protected override Bool OnEval(IEnumerable<Bool> values) {
            foreach (Bool value in values) {
                if (!value.Value) return new(false);
            }
            return new(true);
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "And"+base.ToString();
    }
}
