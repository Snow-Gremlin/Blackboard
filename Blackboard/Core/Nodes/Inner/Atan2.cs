using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This gets the angle whose tangent is the quotient of the two parents.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Atan2"/>
    sealed public class Atan2<T>: Binary<T, T, T>
        where T : IFloatingPoint<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueAdopter<T>, IValueAdopter<T>, Atan2<T>>((input1, input2) => new Atan2<T>(input1, input2));

        /// <summary>Creates an atan2 value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Atan2(IValueAdopter<T> source1 = null, IValueAdopter<T> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>The atan2 of the parents' value during evaluation.</summary>
        /// <param name="value1">The Y value parent.</param>
        /// <param name="value2">The X value parent.</param>
        /// <returns>The atan2 value.</returns>
        protected override T OnEval(T value1, T value2) => value1.Atan2(value2);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Atan2"+base.ToString();
    }
}
