using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>A value node that gets the absolute value of the parent.</summary>
    sealed public class Abs<T>: UnaryValue<T, T>
        where T : ISigned<T>, IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory(value => new Abs<T>(value));

        /// <summary>Creates an absolute value node.</summary>
        public Abs() { }

        /// <summary>Creates an absolute value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        public Abs(IValueParent<T> source = null) : base(source) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Abs<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Abs<T>);

        /// <summary>This will get the absolute value of the parent's value on evaluation.</summary>
        /// <param name="value">The value to get the absolute of.</param>
        /// <returns>The absolute of the given value.</returns>
        protected override T OnEval(T value) => value.Abs();
    }
}
