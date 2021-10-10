using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a bitwise Exclusive OR of two integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    sealed public class BitwiseXor<T>: Nary<T, T>
        where T : IBitwise<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<IValueAdopter<T>, BitwiseXor<T>>((values) => new BitwiseXor<T>(values));

        /// <summary>Creates a bitwise Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseXor(params IValueAdopter<T>[] parents) :
            base(parents) { }

        /// <summary>Creates a bitwise Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public BitwiseXor(IEnumerable<IValueAdopter<T>> parents = null, T value = default) :
            base(parents, value) { }

        /// <summary>Gets the bitwise Exclusive OR of all the parent's booleans.</summary>
        /// <param name="values">The parents to bitwise Exclusive OR together.</param>
        /// <returns>The bitwise Exclusive OR of all the given values.</returns>
        protected override T OnEval(IEnumerable<T> values) =>
            values.Aggregate((left, right) => left.BitwiseOr(right));

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "BitwiseXor"+base.ToString();
    }
}
