using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Determines if the two values are equal with an allowed variance called the epsilon.</summary>
    /// <typeparam name="T">The type being compared.</typeparam>
    sealed public class EpsilonEqual<T>: TernaryValue<T, T, T, Bool>
        where T : ISubtractive<T>, ISigned<T>, IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((left, right, epsilon) => new EpsilonEqual<T>(left, right, epsilon));

        /// <summary>Creates an epsilon equal value node.</summary>
        public EpsilonEqual() { }

        /// <summary>Creates an epsilon equal value node.</summary>
        /// <param name="left">This is the first parent for the left value.</param>
        /// <param name="right">This is the second parent for the right value.</param>
        /// <param name="epsilon">This is the third parent which is used as the epsilon in the comparison.</param>
        public EpsilonEqual(IValueParent<T> left = null, IValueParent<T> right = null, IValueParent<T> epsilon = null) :
            base(left, right, epsilon) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new EpsilonEqual<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(EpsilonEqual<T>);

        /// <summary>Determine if the parent's values are equal during evaluation.</summary>
        /// <param name="left">The first parent's value to compare.</param>
        /// <param name="right">The second parent's value to compare.</param>
        /// <param name="epsilon">The second parent's value to compare.</param>
        /// <returns>True if the two values are equal with the range of the third value, false otherwise.</returns>
        protected override Bool OnEval(T left, T right, T epsilon) => new(left.Sub(right).Abs().CompareTo(epsilon) < 0);
    }
}
