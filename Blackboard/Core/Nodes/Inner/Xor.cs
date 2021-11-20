using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a boolean Exclusive OR of the boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    sealed public class Xor: Nary<Bool, Bool> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<IValueAdopter<Bool>, Xor>((inputs) => new Xor(inputs));

        /// <summary>Creates a boolean Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Xor(params IValueAdopter<Bool>[] parents) :
            base(parents) { }

        /// <summary>Creates a boolean Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Xor(IEnumerable<IValueAdopter<Bool>> parents = null, Bool value = default) :
            base(parents, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Xor";

        /// <summary>Gets the Exclusive OR of all the parent's booleans.</summary>
        /// <param name="values">The parents to Exclusive OR together.</param>
        /// <returns>The Exclusive OR of all the given values.</returns>
        protected override Bool OnEval(IEnumerable<Bool> values) =>
            new(values.Select((b) => b.Value).Aggregate((left, right) => left ^ right));
    }
}
