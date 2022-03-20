using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Determines the minimum integer value from all the parents.</summary>
    sealed public class Min<T>: NaryValue<T, T>
        where T : IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((inputs) => new Min<T>(inputs));

        /// <summary>Creates a minimum value node.</summary>
        public Min() { }

        /// <summary>Creates a minimum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Min(params IValueParent<T>[] parents) : base(parents) { }

        /// <summary>Creates a minimum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Min(IEnumerable<IValueParent<T>> parents = null) : base(parents) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Min<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Min<T>);

        /// <summary>Updates this node's value to the minimum value during evaluation.</summary>
        /// <param name="values">The parents' values to get the min of.</param>
        /// <returns>The minimum value from all the parents.</returns>
        protected override T OnEval(IEnumerable<T> values) => values.Min();

        /// <summary>
        /// The identity element for the node which is a constant
        /// to use when coalescing the node for optimization.
        /// </summary>
        public override IConstant Identity => default(T).ToLiteral();
    }
}
