using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Explicit casts a value node into another value node.</summary>
    sealed public class Explicit<T1, T2>: UnaryValue<T1, T2>
        where T1 : IData
        where T2 : IExplicit<T1, T2>, IEquatable<T2> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((value) => new Explicit<T1, T2>(value));

        /// <summary>Creates a node explicit cast.</summary>
        public Explicit() { }

        /// <summary>Creates a node explicit cast.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Explicit(IValueParent<T1> source = null) : base(source) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Explicit<T1, T2>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Explicit<T1, T2>);

        /// <summary>Gets the value to cast from the parent during evaluation.</summary>
        /// <param name="value">The parent value to cast.</param>
        /// <returns>The cast of the parent value.</returns>
        protected override T2 OnEval(T1 value) => this.Value.CastFrom(value);
    }
}
