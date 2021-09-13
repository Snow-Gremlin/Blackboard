using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a bitwise AND of all the integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/AND.html"/>
    sealed public class BitwiseAnd<T>: Nary<T, T>
        where T : IBitwise<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new FunctionN<IValueAdopter<T>, BitwiseAnd<T>>((values) => new BitwiseAnd<T>(values));

        /// <summary>Creates a bitwise AND value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseAnd(params IValueAdopter<T>[] parents) :
            base(parents) { }

        /// <summary>Creates a bitwise AND value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public BitwiseAnd(IEnumerable<IValueAdopter<T>> parents = null, T value = default) :
            base(parents, value) { }

        /// <summary>Gets the bitwise AND of all the parent's booleans.</summary>
        /// <param name="values">The parents to bitwise AND together.</param>
        /// <returns>The bitwise AND of all the given values.</returns>
        protected override T OnEval(IEnumerable<T> values) =>
            values.Aggregate((left, right) => left.BitwiseAnd(right));

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "BitwiseAnd"+base.ToString();
    }
}
