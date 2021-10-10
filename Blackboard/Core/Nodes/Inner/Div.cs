using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This will divide the first parent value by the second parent value.</summary>
    sealed public class Div<T>: Binary<T, T, T>
        where T : IArithmetic<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncGroup Factory =
            new Function<IValueAdopter<T>, IValueAdopter<T>, Div<T>>((value1, value2) => new Div<T>(value1, value2));

        /// <summary>Creates a divided value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Div(IValueAdopter<T> source1 = null, IValueAdopter<T> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>Gets the first value divided by a second value.</summary>
        /// <param name="value1">The first value to divide.</param>
        /// <param name="value2">The second value to divide.</param>
        /// <returns>The two values divided, or the default if divide by zero.</returns>
        protected override T OnEval(T value1, T value2) => value1.Div(value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Div"+base.ToString();
    }
}
