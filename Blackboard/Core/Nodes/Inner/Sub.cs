using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Gets the difference between the two parent values.</summary>
    sealed public class Sub<T>: BinaryValue<T, T, T>
        where T : ISubtractive<T>, IEquatable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((left, right) => new Sub<T>(left, right));

        /// <summary>Creates a subtraction value node.</summary>
        public Sub() { }

        /// <summary>Creates a subtraction value node.</summary>
        /// <param name="left">This is the first parent for the source value.</param>
        /// <param name="right">This is the second parent for the source value.</param>
        public Sub(IValueParent<T> left = null, IValueParent<T> right = null) : base(left, right) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Sub<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Sub<T>);

        /// <summary>Gets the difference of the parents during evaluation.</summary>
        /// <param name="left">The first value to be subtracted from.</param>
        /// <param name="right">The second value to subtract from the first.</param>
        /// <returns>The difference of the two values.</returns>
        protected override T OnEval(T left, T right) => left.Sub(right);
    }
}
