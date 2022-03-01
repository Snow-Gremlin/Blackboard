using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Determines the maximum integer value from all the parents.</summary>
    sealed public class Max<T>: NaryValue<T, T>
        where T : IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((inputs) => new Max<T>(inputs));

        /// <summary>Creates a maximum value node.</summary>
        public Max() { }

        /// <summary>Creates a maximum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Max(params IValueParent<T>[] parents) : base(parents) { }

        /// <summary>Creates a maximum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Max(IEnumerable<IValueParent<T>> parents = null) : base(parents) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Max<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Max<T>);

        /// <summary>Updates this node's value to the maximum value during evaluation.</summary>
        /// <param name="values">The parents' values to get the max of.</param>
        /// <returns>The maximum value from all the parents.</returns>
        protected override T OnEval(IEnumerable<T> values) => values.Max();

        /// <summary>
        /// The identity element for the node which is a constant
        /// to use when coalescing the node for optimization.
        /// </summary>
        public override IConstant Identity => default(T).ToLiteral();
    }
}
