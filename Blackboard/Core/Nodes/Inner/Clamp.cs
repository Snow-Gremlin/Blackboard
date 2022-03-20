using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This will return the value limited to a range.</summary>
    sealed public class Clamp<T>: TernaryValue<T, T, T, T>
        where T : IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((value, min, max) => new Clamp<T>(value, min, max));

        /// <summary>Creates a clamped value node.</summary>
        public Clamp() { }

        /// <summary>Creates a clamped value node.</summary>
        /// <param name="value">This is the value parent that is clamped.</param>
        /// <param name="min">This is the minimum value parent for the lower edge of the clamp.</param>
        /// <param name="max">This is the maximum value parent for the upper edge of the clamp.</param>
        public Clamp(IValueParent<T> value = null, IValueParent<T> min = null, IValueParent<T> max = null) :
            base(value, min, max) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Clamp<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Clamp<T>);

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
