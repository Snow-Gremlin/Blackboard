using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a right shifts the first parent the amount of the second parent.</summary>
    sealed public class RightShift<T>: Binary<T, T, T>
        where T : IBitwise<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueAdopter<T>, IValueAdopter<T>, RightShift<T>>((left, right) => new RightShift<T>(left, right));

        /// <summary>Creates a right shift value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public RightShift(IValueAdopter<T> source1 = null, IValueAdopter<T> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "RightShift";

        /// <summary>Right shifts the value during evaluation.</summary>
        /// <param name="value1">The integer value to right shift.</param>
        /// <param name="value2">The integer value to right shift the other value by.</param>
        /// <returns>The right shifted value for this node.</returns>
        protected override T OnEval(T value1, T value2) => value1.RightShift(value2);
    }
}
