using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Gets the difference between the two parent values.</summary>
    sealed public class Sub<T>: Binary<T, T, T>
        where T : IArithmetic<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueParent<T>, IValueParent<T>, Sub<T>>((left, right) => new Sub<T>(left, right));

        /// <summary>Creates a subtraction value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Sub(IValueParent<T> source1 = null, IValueParent<T> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Sub";

        /// <summary>Gets the difference of the parents during evaluation.</summary>
        /// <param name="value1">The first value to be subtracted from.</param>
        /// <param name="value2">The second value to subtract from the first.</param>
        /// <returns>The difference of the two values.</returns>
        protected override T OnEval(T value1, T value2) => value1.Sub(value2);
    }
}
