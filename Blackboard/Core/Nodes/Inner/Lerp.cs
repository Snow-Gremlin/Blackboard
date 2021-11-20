using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This will return the linear interpolation between two parent values.</summary>
    sealed public class Lerp<T>: Ternary<T, T, T, T>
        where T : IFloatingPoint<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueAdopter<T>, IValueAdopter<T>, IValueAdopter<T>, Lerp<T>>(
                (input1, input2, input3) => new Lerp<T>(input1, input2, input3));

        /// <summary>Creates a linear interpolation value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="source3">This is the third parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Lerp(IValueAdopter<T> source1 = null, IValueAdopter<T> source2 = null,
            IValueAdopter<T> source3 = null, T value = default) :
            base(source1, source2, source3, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Lerp";

        /// <summary>Selects the value to return during evaluation.</summary>
        /// <param name="value">
        /// The value from the first parent. This is the iterator value.
        /// Zero or less will return the value from the min parent.
        /// One or more will return the value from the max parent.
        /// Between zero and one will return the linear interpolation.
        /// </param>
        /// <param name="min">The minimum value to return.</param>
        /// <param name="max">The maximum value to return.</param>
        /// <returns>The lerp value to set to this node.</returns>
        protected override T OnEval(T value, T min, T max) => value.Lerp(min, max);
    }
}
