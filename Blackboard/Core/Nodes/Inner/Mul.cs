using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This gets the product of all the double parents.</summary>
    sealed public class Mul<T>: NaryValue<T, T>
        where T : IMultiplicative<T>, IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((inputs) => new Mul<T>(inputs));

        /// <summary>Creates a product value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Mul(params IValueParent<T>[] parents) : base(parents) { }

        /// <summary>Creates a product value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Mul(IEnumerable<IValueParent<T>> parents = null) : base(parents) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Mul";

        /// <summary>Gets the product of the parent values during evaluation.</summary>
        /// <param name="values">All the parent values to multiply.</param>
        /// <returns>The product of the parent values.</returns>
        protected override T OnEval(IEnumerable<T> values) => default(T).Mul(values);

        /// <summary>
        /// The identity element for the node which is a constant
        /// to use when coalescing the node for optimization.
        /// </summary>
        public override IConstant Identity => default(T).MulIdentityValue.ToLiteral();

        /// <summary>Indicates that the parents can be reordered.</summary>
        public override bool Commutative => default(T).MulCommutable;
    }
}
