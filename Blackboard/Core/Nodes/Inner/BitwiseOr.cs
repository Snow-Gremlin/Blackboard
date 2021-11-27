using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a bitwise OR of all the integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/OR.html"/>
    sealed public class BitwiseOr<T>: NaryValue<T, T>
        where T : IBitwise<T>, IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((values) => new BitwiseOr<T>(values));

        /// <summary>Creates a bitwise OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseOr(params IValueParent<T>[] parents) : base(parents) { }

        /// <summary>Creates a bitwise OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseOr(IEnumerable<IValueParent<T>> parents = null) : base(parents) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "BitwiseOr";

        /// <summary>Gets the bitwise OR of all the parent's booleans.</summary>
        /// <param name="values">The parents to bitwise OR together.</param>
        /// <returns>The bitwise OR of all the given values.</returns>
        protected override T OnEval(IEnumerable<T> values) => values.Aggregate((left, right) => left.BitwiseOr(right));
    }
}
