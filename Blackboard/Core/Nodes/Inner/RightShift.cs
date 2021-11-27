using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a right shifts the first parent the amount of the second parent.</summary>
    sealed public class RightShift<T>: BinaryValue<T, T, T>
        where T : IBitwise<T>, IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((left, right) => new RightShift<T>(left, right));

        /// <summary>Creates a right shift value node.</summary>
        /// <param name="left">This is the first parent for the source value.</param>
        /// <param name="right">This is the second parent for the source value.</param>
        public RightShift(IValueParent<T> left = null, IValueParent<T> right = null) : base(left, right) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "RightShift";

        /// <summary>Right shifts the value during evaluation.</summary>
        /// <param name="left">The integer value to right shift.</param>
        /// <param name="right">The integer value to right shift the other value by.</param>
        /// <returns>The right shifted value for this node.</returns>
        protected override T OnEval(T left, T right) => left.RightShift(right);
    }
}
