using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This will get the modulo the first parent value by the second parent value.</summary>
    sealed public class Mod<T>: Binary<T, T, T>
        where T : IArithmetic<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new Function<IValue<T>, IValue<T>>((left, right) => new Mod<T>(left, right));

        /// <summary>Creates a modulo value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Mod(IValue<T> source1 = null, IValue<T> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>Gets the first value moduled by a second value.</summary>
        /// <param name="value1">The first value to modulo.</param>
        /// <param name="value2">The second value to modulo.</param>
        /// <returns>The two values moduled, or the default if moduled by zero.</returns>
        protected override T OnEval(T value1, T value2) => value1.Mod(value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Mod"+base.ToString();
    }
}
