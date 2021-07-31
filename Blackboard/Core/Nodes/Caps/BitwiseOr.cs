using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a bitwise OR of all the integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/OR.html"/>
    sealed public class BitwiseOr<T>: Nary<T, T>
        where T : IBitwise<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory = new FunctionN<IValue<T>>((values) => new BitwiseOr<T>(values));

        /// <summary>Creates a bitwise OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseOr(params IValue<T>[] parents) :
            base(parents) { }

        /// <summary>Creates a bitwise OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public BitwiseOr(IEnumerable<IValue<T>> parents = null, T value = default) :
            base(parents, value) { }

        /// <summary>Gets the bitwise OR of all the parent's booleans.</summary>
        /// <param name="values">The parents to bitwise OR together.</param>
        /// <returns>The bitwise OR of all the given values.</returns>
        protected override T OnEval(IEnumerable<T> values) =>
            values.Aggregate((left, right) => left.BitwiseOr(right));

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "BitwiseOr"+base.ToString();
    }
}
