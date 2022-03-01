using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Determines if the two values are less than or equal.</summary>
    /// <typeparam name="T">The type being compared.</typeparam>
    sealed public class LessThanOrEqual<T>: BinaryValue<T, T, Bool>
        where T : IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((left, right) => new LessThanOrEqual<T>(left, right));

        /// <summary>Creates a less than or equal value node.</summary>
        public LessThanOrEqual() { }

        /// <summary>Creates a less than or equal value node.</summary>
        /// <param name="left">This is the first parent for the source value.</param>
        /// <param name="right">This is the second parent for the source value.</param>
        public LessThanOrEqual(IValueParent<T> left = null, IValueParent<T> right = null) : base(left, right) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new LessThanOrEqual<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(LessThanOrEqual<T>);

        /// <summary>Determine if the parent's values are less than or equal during evaluation.</summary>
        /// <param name="left">The first parent's value to compare.</param>
        /// <param name="right">The second parent's value to compare.</param>
        /// <returns>True if the first value is less than or equal than the second value, false otherwise.</returns>
        protected override Bool OnEval(T left, T right) => new(left.CompareTo(right) <= 0);
    }
}
