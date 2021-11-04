using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This gets the log of the first value with the base of the second value.</summary>
    sealed public class Log<T>: Binary<T, T, T>
        where T : IFloatingPoint<T>, IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueAdopter<T>, IValueAdopter<T>, Log<T>>((input1, input2) => new Log<T>(input1, input2));

        /// <summary>Creates a log value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Log(IValueAdopter<T> source1 = null, IValueAdopter<T> source2 = null, T value = default) :
            base(source1, source2, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Log";

        /// <summary>The log of the parents' value during evaluation.</summary>
        /// <param name="value1">The value to get the log of.</param>
        /// <param name="value2">The new base of the log.</param>
        /// <returns>The atan2 value.</returns>
        protected override T OnEval(T value1, T value2) => value1.Log(value2);
    }
}
