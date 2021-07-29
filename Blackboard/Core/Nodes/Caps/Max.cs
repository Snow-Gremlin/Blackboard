using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Determinines the maximum integer value from all the parents.</summary>
    sealed public class Max<T>: Nary<T, T>
        where T : IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory = new FuncN<IValue<T>>((inputs) => new Max<T>(inputs));

        /// <summary>Creates a maximum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Max(params IValue<T>[] parents) :
            base(parents) { }

        /// <summary>Creates a maximum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Max(IEnumerable<IValue<T>> parents = null, T value = default) :
            base(parents, value) { }

        /// <summary>Updates this node's value to the maximum value during evaluation.</summary>
        /// <param name="values">The parents' values to get the max of.</param>
        /// <returns>The maximum value from all the parents.</returns>
        protected override T OnEval(IEnumerable<T> values) =>
            values.Aggregate((T left, T right) => left.CompareTo(right) > 0 ? left : right);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Max"+base.ToString();
    }
}
