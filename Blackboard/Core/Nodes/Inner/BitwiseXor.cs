using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a bitwise Exclusive OR of two integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    sealed public class BitwiseXor<T>: NaryValue<T, T>
        where T : IBitwise<T>, IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((values) => new BitwiseXor<T>(values));

        /// <summary>Creates a bitwise Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseXor(params IValueParent<T>[] parents) : base(parents) { }

        /// <summary>Creates a bitwise Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseXor(IEnumerable<IValueParent<T>> parents = null) : base(parents) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "BitwiseXor";

        /// <summary>Gets the bitwise Exclusive OR of all the parent's booleans.</summary>
        /// <param name="values">The parents to bitwise Exclusive OR together.</param>
        /// <returns>The bitwise Exclusive OR of all the given values.</returns>
        protected override T OnEval(IEnumerable<T> values) => values.Aggregate((left, right) => left.BitwiseOr(right));
    }
}
