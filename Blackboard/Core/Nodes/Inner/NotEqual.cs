using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Determines if the two values are not equal.</summary>
    /// <typeparam name="T">The type being compared.</typeparam>
    sealed public class NotEqual<T>: BinaryValue<T, T, Bool>
        where T : IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((left, right) => new NotEqual<T>(left, right));

        /// <summary>Creates a not equal value node.</summary>
        /// <param name="left">This is the first parent for the source value.</param>
        /// <param name="right">This is the second parent for the source value.</param>
        public NotEqual(IValueParent<T> left = null, IValueParent<T> right = null) : base(left, right) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "NotEqual";

        /// <summary>Determine if the parent's values are not equal during evaluation.</summary>
        /// <param name="left">The first parent's value to compare.</param>
        /// <param name="right">The second parent's value to compare.</param>
        /// <returns>True if the two values are not equal, false otherwise.</returns>
        protected override Bool OnEval(T left, T right) => new(!left.Equals(right));
    }
}
