using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This will determine if the given value is inclusively with a range.</summary>
    sealed public class InRange<T>: TernaryValue<T, T, T, Bool>
        where T : IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((value, min, max) => new InRange<T>(value, min, max));

        /// <summary>Creates an in range value node.</summary>
        public InRange() { }

        /// <summary>Creates an in range value node.</summary>
        /// <param name="value">This is the value parent that is checked against the range.</param>
        /// <param name="min">This is the minimum value parent for the lower edge of the range.</param>
        /// <param name="max">This is the maximum value parent for the upper edge of the range.</param>
        public InRange(IValueParent<T> value = null, IValueParent<T> min = null, IValueParent<T> max = null) :
            base(value, min, max) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new InRange<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(InRange<T>);

        /// <summary>Selects the value to return during evaluation.</summary>
        /// <param name="value">
        /// The value from the first parent. This is the input value to test.
        /// If less than the min value or greater than the max value
        /// then false is returned, otherwise true is returned.
        /// </param>
        /// <param name="min">The minimum value to return.</param>
        /// <param name="max">The maximum value to return.</param>
        /// <returns>True if in the inclusive range and false otherwise.</returns>
        protected override Bool OnEval(T value, T min, T max) => new(value.InRange(min, max));
    }
}
