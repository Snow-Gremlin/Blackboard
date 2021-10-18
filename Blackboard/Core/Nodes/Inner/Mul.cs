using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This gets the product of all the double parents.</summary>
    sealed public class Mul<T>: Nary<T, T>
        where T : IArithmetic<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<IValueAdopter<T>, Mul<T>>((inputs) => new Mul<T>(inputs));

        /// <summary>Creates a product value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Mul(params IValueAdopter<T>[] parents) :
            base(parents) { }

        /// <summary>Creates a product value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Mul(IEnumerable<IValueAdopter<T>> parents = null, T value = default) :
            base(parents, value) { }

        /// <summary>Gets the product of the parent values during evaluation.</summary>
        /// <param name="values">All the parent values to multiply.</param>
        /// <returns>The product of the parent values.</returns>
        protected override T OnEval(IEnumerable<T> values) =>
            values.Aggregate((T left, T right) => left.Mul(right));

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Mul"+base.ToString();
    }
}
