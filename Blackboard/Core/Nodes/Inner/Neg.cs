using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Negates the given parents value.</summary>
    sealed public class Neg<T>: Unary<T, T>
        where T : IArithmetic<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueAdopter<T>, Neg<T>>((input) => new Neg<T>(input));

        /// <summary>Creates a negated value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Neg(IValueAdopter<T> source = null, T value = default) :
            base(source, value) { }

        /// <summary>Gets the negated value of the parent during evaluation.</summary>
        /// <param name="value">The parent value to negate.</param>
        /// <returns>The negated parent value.</returns>
        protected override T OnEval(T value) => value.Neg();

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Neg"+base.ToString();
    }
}
