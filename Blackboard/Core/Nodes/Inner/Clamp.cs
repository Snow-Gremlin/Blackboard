using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This will return the value limitted to a range.</summary>
    sealed public class Clamp<T>: TernaryValue<T, T, T, T>
        where T : IArithmetic<T>, IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueParent<T>, IValueParent<T>, IValueParent<T>, Clamp<T>>(
                (value1, value2, value3) => new Clamp<T>(value1, value2, value3));

        /// <summary>Creates a clamped value node.</summary>
        /// <param name="source1">This is the value parent that is clamped.</param>
        /// <param name="source2">This is the minimum value parent for the lower edge of the clamp.</param>
        /// <param name="source3">This is the maximum value parent for the upper edge of the clamp.</param>
        /// <param name="value">The default value for this node.</param>
        public Clamp(IValueParent<T> source1 = null, IValueParent<T> source2 = null,
            IValueParent<T> source3 = null, T value = default) :
            base(source1, source2, source3, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Clamp";

        /// <summary>Selects the value to return during evaluation.</summary>
        /// <param name="value">
        /// The value from the first parent. This is the input value to clamp.
        /// If less than the min value then the min value is returned.
        /// If greater than the max value then the max value is returned.
        /// Otherwise the input value is returned when between the other two values.
        /// </param>
        /// <param name="min">The minimum value to return.</param>
        /// <param name="max">The maximum value to return.</param>
        /// <returns>The clamped value to set to this node.</returns>
        protected override T OnEval(T value, T min, T max) => value.Clamp(min, max);
    }
}
