using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a bitwise Exclusive OR of two integer parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/XOR.html"/>
    sealed public class BitwiseXor<T>: NaryValue<T, T>
        where T : IBitwise<T>, IEquatable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((values) => new BitwiseXor<T>(values));

        /// <summary>Creates a bitwise Exclusive OR value node.</summary>
        public BitwiseXor() { }

        /// <summary>Creates a bitwise Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseXor(params IValueParent<T>[] parents) : base(parents) { }

        /// <summary>Creates a bitwise Exclusive OR value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public BitwiseXor(IEnumerable<IValueParent<T>> parents = null) : base(parents) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new BitwiseXor<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(BitwiseXor<T>);

        /// <summary>Gets the bitwise Exclusive OR of all the parent's booleans.</summary>
        /// <param name="values">The parents to bitwise Exclusive OR together.</param>
        /// <returns>The bitwise Exclusive OR of all the given values.</returns>
        protected override T OnEval(IEnumerable<T> values) => new T().BitwiseXor(values);

        /// <summary>
        /// The identity element for the node which is a constant
        /// to use when coalescing the node for optimization.
        /// </summary>
        public override IConstant Identity => default(T).OrIdentityValue.ToLiteral();
    }
}
