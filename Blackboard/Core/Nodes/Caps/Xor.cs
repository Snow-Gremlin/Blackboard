using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a boolean Exclusive OR of the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    sealed public class Xor: Nary<Bool, Bool> {

        /// <summary>Creates a boolean Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Xor(params IValue<Bool>[] parents) :
            base(parents) { }

        /// <summary>Creates a boolean Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Xor(IEnumerable<IValue<Bool>> parents = null, Bool value = default) :
            base(parents, value) { }

        /// <summary>Gets the Exclusive OR of all the parent's booleans.</summary>
        /// <param name="values">The to Exclusive OR together.</param>
        /// <returns>The Exclusive OR of all the given values.</returns>
        protected override Bool OnEval(IEnumerable<Bool> values) =>
            Bool.Wrap(values.Select((b) => b.Value).Aggregate((left, right) => left ^ right));

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Xor"+base.ToString();
    }
}
