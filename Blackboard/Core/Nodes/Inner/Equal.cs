using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Determines if the two values are equal.</summary>
    /// <typeparam name="T">The type being compared.</typeparam>
    sealed public class Equal<T>: Binary<T, T, Bool>
        where T : IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncGroup Factory =
            new Function<IValueAdopter<T>, IValueAdopter<T>, Equal<T>>((left, right) => new Equal<T>(left, right));

        /// <summary>Creates an equal value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Equal(IValueAdopter<T> source1 = null, IValueAdopter<T> source2 = null, Bool value = default) :
            base(source1, source2, value) { }

        /// <summary>Determine if the parent's values are equal during evaluation.</summary>
        /// <param name="value1">The first parent's value to compare.</param>
        /// <param name="value2">The second parent's value to compare.</param>
        /// <returns>True if the two values are equal, false otherwise.</returns>
        protected override Bool OnEval(T value1, T value2) => new(value1.CompareTo(value2) == 0);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Equal"+base.ToString();
    }
}
