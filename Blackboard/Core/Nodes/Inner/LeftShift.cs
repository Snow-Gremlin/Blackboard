using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a left shifts the first parent the amount of the second parent.</summary>
    sealed public class LeftShift<T>: BinaryValue<T, T, T>
        where T : IBitwise<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueParent<T>, IValueParent<T>, LeftShift<T>>((left, right) => new LeftShift<T>(left, right));

        /// <summary>Creates a left shift value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public LeftShift(IValueParent<T> source1 = null, IValueParent<T> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "LeftShift";

        /// <summary>Left shifts the value during evaluation.</summary>
        /// <param name="value1">The value to left shift.</param>
        /// <param name="value2">The value to left shift the other value by.</param>
        /// <returns>The left shifted value for this node.</returns>
        protected override T OnEval(T value1, T value2) => value1.LeftShift(value2);
    }
}
