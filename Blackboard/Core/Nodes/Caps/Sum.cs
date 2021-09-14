using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Gets the sum of all of the parent values.</summary>
    sealed public class Sum<T>: Nary<T, T>
        where T : IAdditive<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        /// <param name="needOneNoCast">
        /// Indicates that at least one argument must not be a cast.
        /// This is used for things like String where all types can implicit cast to a string.
        /// </param>
        static public IFunction Factory(bool needOneNoCast = false) => 
            new FunctionN<IValueAdopter<T>, Sum<T>>((inputs) => new Sum<T>(inputs), needOneNoCast: needOneNoCast);

        /// <summary>Creates a sum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Sum(params IValueAdopter<T>[] parents) :
            base(parents) { }

        /// <summary>Creates a sum value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Sum(IEnumerable<IValueAdopter<T>> parents = null, T value = default) :
            base(parents, value) { }

        /// <summary>Gets the sum of all the parent values.</summary>
        /// <param name="values">The values to sum together.</param>
        /// <returns>The sum of the parent values.</returns>
        protected override T OnEval(IEnumerable<T> values) =>
            values.Aggregate((left, right) => left.Sum(right));

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Sum"+base.ToString();
    }
}
