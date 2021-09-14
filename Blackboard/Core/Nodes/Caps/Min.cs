using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Determinines the minimum integer value from all the parents.</summary>
    sealed public class Min<T>: Nary<T, T>
        where T : IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new FunctionN<IValueAdopter<T>, Min<T>>((inputs) => new Min<T>(inputs));

        /// <summary>Creates a minimum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Min(params IValueAdopter<T>[] parents) :
            base(parents) { }

        /// <summary>Creates a minimum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Min(IEnumerable<IValueAdopter<T>> parents = null, T value = default) :
            base(parents, value) { }

        /// <summary>Updates this node's value to the minimum value during evaluation.</summary>
        /// <param name="values">The parents' values to get the min of.</param>
        /// <returns>The minimum value from all the parents.</returns>
        protected override T OnEval(IEnumerable<T> values) =>
            values.Aggregate((T left, T right) => left.CompareTo(right) < 0 ? left : right);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Min"+base.ToString();
    }
}
