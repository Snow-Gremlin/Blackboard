using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>Determinines the minimum integer value from all the parents.</summary>
    sealed public class Min<T>: Nary<T, T> where T : System.IComparable<T> {

        /// <summary>Creates a minimum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Min(params IValue<T>[] parents) :
            base(parents) { }

        /// <summary>Creates a minimum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Min(IEnumerable<IValue<T>> parents = null, T value = default) :
            base(parents, value) { }

        /// <summary>Updates this node's value to the minimum value during evaluation.</summary>
        /// <param name="values">The parents' values to get the min of.</param>
        /// <returns>The minimum value from all the parents.</returns>
        protected override T OnEval(IEnumerable<T> values) {
            bool first = true;
            T result = default;
            foreach (T value in values) {
                if (first) {
                    result = value;
                    first = false;
                }
                if (value.CompareTo(result) < 0) result = value;
            }
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Min"+base.ToString();
    }
}
