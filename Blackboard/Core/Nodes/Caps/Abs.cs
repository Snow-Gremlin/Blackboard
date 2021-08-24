using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>A value node that gets the absolute value of the parent.</summary>
    sealed public class Abs<T>: Unary<T, T>
        where T : IArithmetic<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new Function<IValue<T>, Abs<T>>((value) => new Abs<T>(value));

        /// <summary>Creates an absolute value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Abs(IValue<T> source = null, T value = default) :
            base(source, value) { }

        /// <summary>This will get the absolute value of the parent's value on evaluation.</summary>
        /// <param name="value">The value to get the absolute of.</param>
        /// <returns>The absolute of the given value.</returns>
        protected override T OnEval(T value) => value.Abs();

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Abs"+base.ToString();
    }
}
